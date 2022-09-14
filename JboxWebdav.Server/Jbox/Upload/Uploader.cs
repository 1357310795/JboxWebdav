using Jbox;
using Jbox.Models;
using Jbox.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.Server.Jbox.Upload
{
    public class Uploader
    {
        public long length { get; set; }
        public Stream stream { get; set; }
        public string targetPath { get; set; }
        public int maxretrytime { get; set; } = 3;
        public int threadcount { get; set; } = 4;
        public int runningcount { get; set; } = 0;
        
        private TaskCompletionSource<CommonResult> taskCompletionSource;
        private int count, current;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        private string[] hashes;

        public Uploader(string targetPath, Stream stream, long length)
        {
            this.length = length;
            this.stream = stream;
            this.targetPath = targetPath;
            count = (int)(length / (4 * 1024 * 1024));
            if (length > count * 4 * 1024 * 1024)
                count++;
            current = 0;
            hashes = new string[count + 10];
        }

        public CommonResult Run()
        {
            Console.WriteLine("Uploader Started!");
            taskCompletionSource = new TaskCompletionSource<CommonResult>();
            for(int i = 0; i < threadcount; i++)
            {
                CheckForNext();
            }
            taskCompletionSource.Task.Wait();
            var res1 = taskCompletionSource.Task.Result;
            Console.WriteLine($"Upload All Done, Result: {res1.success}");
            if (!res1.success)
                return res1;

            var res2 = UploadFinal(targetPath, length);
            Console.WriteLine($"Upload Final Done, Result: {res2.success}");
            if (!res2.success)
                return res2;
            if (res2.code != System.Net.HttpStatusCode.OK)
                return new CommonResult(false, res2.result);
            return new CommonResult(true, "");
        }

        public void CheckForNext()
        {
            if (current >= count)
            {
                if (runningcount == 0)
                    taskCompletionSource.TrySetResult(new CommonResult(true, ""));
                return;
            }

            _lock.Wait();
            try
            {
                runningcount++;
                current++;
                Console.WriteLine($"Task Chunk {current} Assigned");
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += Bgw_DoWork;
                bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;

                ChunkFileUploadTaskArgs args = new ChunkFileUploadTaskArgs();
                args.retrytimes = maxretrytime;
                args.length = (int)(current == count ? length - (current - 1) * 4 * 1024 * 1024 : 4 * 1024 * 1024);
                args.id = current;

                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                int bytesToRead = args.length;
                while ((bytesRead = stream.Read(buffer, 0, Math.Min(buffer.Length, bytesToRead))) != 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                    ms.Flush();
                    bytesToRead -= bytesRead;
                }

                args.stream = ms;
                bgw.RunWorkerAsync(args);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetResult(new CommonResult(false, ex.Message));
            }
            _lock.Release();
        }

        private void Bgw_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            ChunkFileUploadTaskArgs args = (ChunkFileUploadTaskArgs)e.Result;
            Console.WriteLine($"Task Chunk {args.id} {(args.success ? "Succeeded" : "Failed")}");
            if (!args.success)
            {
                taskCompletionSource.TrySetResult(new CommonResult(false, "上传失败"));
                return;
            }
            hashes[args.id] = args.hashcode;
            runningcount--;
            CheckForNext();
        }

        private void Bgw_DoWork(object? sender, DoWorkEventArgs e)
        {
            ChunkFileUploadTaskArgs args = (ChunkFileUploadTaskArgs)e.Argument;
            Console.WriteLine($"Task Chunk {args.id} Started");
            while (args.retrytimes-- > 0)
            {
                try
                {
                    args.hashcode = Common.GetSHA256(args.stream);
                }
                catch (Exception ex)
                {
                    continue;
                }
                var res1 = CheckPart(args.hashcode);
                if (!res1.success)
                    continue;
                if (res1.code != System.Net.HttpStatusCode.OK)
                    continue;
                CommitDto commit = null;
                try
                {
                    commit = JsonConvert.DeserializeObject<CommitDto>(res1.result);
                }
                catch(Exception ex)
                {
                    continue;
                }
                if (commit.needed_block.Count == 0)
                {
                    args.success = true;
                    e.Result = args;
                    return;
                }
                var res2 = UploadPart(args.stream, args.length, commit.needed_block[0], commit.upload_id);
                if (!res2.success)
                    continue;
                if (res2.code != System.Net.HttpStatusCode.OK)
                    continue;

                args.success = true;
                e.Result = args;
                return;
            }
            args.success = false;
            e.Result = args;
            return;
        }

        private WebResult CheckPart(string hash)
        {
            var headers = JboxService.GetCommonHeaders();
            headers.Add("Origin", "https://jbox.sjtu.edu.cn");
            var paras = JboxService.GetCommonQueryParas();
            var form = new Dictionary<string, string>();
            form.Add("hashes[]", hash);

            string url = "https://jbox.sjtu.edu.cn:10081/v2/commit_chunked_upload/commit/databox";
            var res = Web.Post(url, paras, headers, form, false);
            return res;
        }

        private WebResult UploadPart(Stream stream, int length, string hash, string uploadid)
        {
            var headers = JboxService.GetCommonHeaders();
            headers.Add("Origin", "https://jbox.sjtu.edu.cn");
            headers["Content-Type"] = "application/octet-stream";
            var paras = JboxService.GetCommonQueryParas();
            paras.Add("hash", hash);
            paras.Add("offset", "0");
            paras.Add("upload_id", uploadid);

            string url = "https://jbox.sjtu.edu.cn:10081/v2/chunked_upload";
            var res = Web.Post(url, paras, headers, stream);
            return res;
        }

        private WebResult UploadFinal(string path, long length)
        {
            var headers = JboxService.GetCommonHeaders();
            headers.Add("Origin", "https://jbox.sjtu.edu.cn");
            var paras = JboxService.GetCommonQueryParas();
            var form = new Dictionary<string, string>();
            form.Add("bytes", length.ToString());
            form.Add("is_file_commit", "true");
            form.Add("language", "zh");
            form.Add("overwrite", "true");
            form.Add("path", path.UrlEncodeByParts());
            form.Add("path_type", "self");
            form.Add("utime", Common.GetTimeStampMilli());
            var formstr = Web.BuildForm(form, false);
            for (int i = 1; i <= count; i++)
                formstr += $"&hashes[]={hashes[i]}";

            string url = "https://jbox.sjtu.edu.cn:10081/v2/commit_chunked_upload/commit/databox";
            url += path.UrlEncodeByParts();

            var res = Web.Post(url, paras, headers, formstr, false);
            return res;
        }
    }

    public class ChunkFileUploadTaskArgs
    {
        public int length { get; set; }
        public Stream stream { get; set; }
        public int retrytimes { get; set; }
        public int id { get; set; }
        public string hashcode { get; set; }
        public bool success { get; set; }
    }

    public class CommitDto
    {
        public string upload_id { get; set; }
        public List<string> needed_block { get; set; }
    }
}
