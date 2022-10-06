using Newtonsoft.Json;
using NWebDav.Server.Logging;
using NWebDav.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using NWebDav.Server.Helpers;

namespace JboxWebdav.Server.Jbox
{
    public class JboxSharedItemInfo
    {
        [JsonProperty("access_mode")]
        public int AccessMode { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        [JsonProperty("client_mtime")]
        public string ClientMtime { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("content")]
        public JboxSharedItemInfo[] Content { get; set; }

        [JsonProperty("content_size")]
        public long ContentSize { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("data_url")]
        public string DataUrl { get; set; }

        [JsonProperty("delivery_code")]
        public string DeliveryCode { get; set; }

        [JsonProperty("delivery_creator")]
        public string DeliveryCreator { get; set; }

        [JsonProperty("delivery_creator_uid")]
        public long DeliveryCreatorUid { get; set; }

        [JsonProperty("delivery_desc")]
        public string DeliveryDesc { get; set; }

        [JsonProperty("delivery_mtime")]
        public string DeliveryMtime { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("expiration")]
        public long Expiration { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("is_checked_watermark")]
        public bool IsCheckedWatermark { get; set; }

        [JsonProperty("is_dir")]
        public bool IsDir { get; set; }

        [JsonProperty("is_open_watermark")]
        public bool IsOpenWatermark { get; set; }

        [JsonProperty("is_shared")]
        public bool IsShared { get; set; }

        [JsonProperty("metadata_url")]
        public string MetadataUrl { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("neid")]
        public string Neid { get; set; }

        [JsonProperty("nsid")]
        public long Nsid { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("path_type")]
        public string PathType { get; set; }

        [JsonProperty("preview_delivery_url")]
        public string PreviewDeliveryUrl { get; set; }

        [JsonProperty("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonProperty("rev")]
        public string Rev { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("support_preview")]
        public string SupportPreview { get; set; }

        [JsonProperty("total_size")]
        public long TotalSize { get; set; }

        [JsonProperty("updator")]
        public string Updator { get; set; }

        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }

        [JsonProperty("watermarkDownloadUrl")]
        public string WatermarkDownloadUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public string Token { get; set; }
        public bool IsDetailed = true;
        public bool success
        {
            get
            {
                return Type != "error";
            }
        }
        internal string GetName()
        {
            if (IsDir)
            {
                var tmp = Path.Last() == '/' ? Path.Substring(0, Path.Length - 1) : Path;
                return tmp.Substring(tmp.LastIndexOf("/") + 1);
            }
            else
            {
                return Path.Substring(Path.LastIndexOf("/") + 1);
            }
        }

        internal IEnumerable<JboxSharedItemInfo> GetDirectories()
        {
            if (!IsDetailed)
                MergeResults(JboxService.GetJboxSharedItemInfo(DeliveryCode, UriHelper.RemoveTopFolder(Path), Token));
            foreach (var item in Content)
            {
                if (item.IsDir)
                {
                    var tmp = item;
                    tmp.IsDetailed = false;
                    yield return tmp;
                }
            }
        }

        internal IEnumerable<JboxSharedItemInfo> GetFiles()
        {
            if (!IsDetailed)
                MergeResults(JboxService.GetJboxSharedItemInfo(DeliveryCode, UriHelper.RemoveTopFolder(Path), Token));
            foreach (var item in Content)
            {
                if (!item.IsDir)
                {
                    var tmp = item;
                    tmp.IsDetailed = false;
                    yield return tmp;
                }
            }
        }

        private void MergeResults(JboxSharedItemInfo info)
        {
            if (!info.success)
                return;
            this.Content = info.Content;
            this.IsDetailed = true;
        }

        internal Stream OpenRead()
        {
            return JboxService.GetSharedFile(this);
        }

        internal Stream OpenRead(long start, long end)
        {
            return JboxService.GetSharedFile(this, start, end);
        }
    }

    public class JboxUserInfo
    {
        [JsonProperty("account_id")]
        public long AccountId { get; set; }

        [JsonProperty("cloud_allow_scan")]
        public long CloudAllowScan { get; set; }

        [JsonProperty("cloud_allow_share")]
        public long CloudAllowShare { get; set; }

        [JsonProperty("cloud_quota")]
        public long CloudQuota { get; set; }

        [JsonProperty("cloud_used")]
        public long CloudUsed { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("ctime")]
        public string Ctime { get; set; }

        [JsonProperty("delivery_support")]
        public bool DeliverySupport { get; set; }

        [JsonProperty("docs_limit_enable")]
        public long DocsLimitEnable { get; set; }

        [JsonProperty("download_threshold_speed")]
        public long DownloadThresholdSpeed { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_chk")]
        public bool EmailChk { get; set; }

        [JsonProperty("from_domain_account")]
        public bool FromDomainAccount { get; set; }

        [JsonProperty("is_beyond_docsLimit")]
        public bool IsBeyondDocsLimit { get; set; }

        [JsonProperty("link_sharing_enable")]
        public long LinkSharingEnable { get; set; }

        [JsonProperty("local_edit_switch")]
        public bool LocalEditSwitch { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("mobile_chk")]
        public bool MobileChk { get; set; }

        [JsonProperty("ne_manage")]
        public long NeManage { get; set; }

        [JsonProperty("netzone_enable")]
        public long NetzoneEnable { get; set; }

        [JsonProperty("netzone_list")]
        public string NetzoneList { get; set; }

        [JsonProperty("password_changeable")]
        public bool PasswordChangeable { get; set; }

        [JsonProperty("personal_sharing_enable")]
        public long PersonalSharingEnable { get; set; }

        [JsonProperty("photo")]
        public string[] Photo { get; set; }

        [JsonProperty("preview_support")]
        public bool PreviewSupport { get; set; }

        [JsonProperty("quota")]
        public long Quota { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("region_desc")]
        public string RegionDesc { get; set; }

        [JsonProperty("region_id")]
        public long RegionId { get; set; }

        [JsonProperty("role")]
        public long Role { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("uid")]
        public long Uid { get; set; }

        [JsonProperty("upload_threshold_speed")]
        public long UploadThresholdSpeed { get; set; }

        [JsonProperty("use_cloud_quota")]
        public long UseCloudQuota { get; set; }

        [JsonProperty("use_local_quota")]
        public long UseLocalQuota { get; set; }

        [JsonProperty("use_threshold")]
        public long UseThreshold { get; set; }

        [JsonProperty("used")]
        public long Used { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("user_slug")]
        public string UserSlug { get; set; }

        [JsonProperty("valid_enable")]
        public long ValidEnable { get; set; }

        [JsonProperty("valid_end_time")]
        public string ValidEndTime { get; set; }

        [JsonProperty("valid_start_time")]
        public string ValidStartTime { get; set; }
    }

    public class JboxDirectoryInfo
    {
        #region Json Properties
        [JsonProperty("access_mode")]
        public long AccessMode { get; set; }

        [JsonProperty("approveable")]
        public bool Approveable { get; set; }

        [JsonProperty("authable")]
        public bool Authable { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        [JsonProperty("content")]
        public JboxItemInfo[] Content { get; set; }

        [JsonProperty("content_size")]
        public long ContentSize { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("creator_uid")]
        public long CreatorUid { get; set; }

        [JsonProperty("delivery_code")]
        public string DeliveryCode { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("dir_type")]
        public long DirType { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("from_name")]
        public string FromName { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("is_bookmark")]
        public bool IsBookmark { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("is_dir")]
        public bool IsDir { get; set; }

        [JsonProperty("is_display")]
        public bool IsDisplay { get; set; }

        [JsonProperty("is_group")]
        public bool IsGroup { get; set; }

        [JsonProperty("is_shared")]
        public bool IsShared { get; set; }

        [JsonProperty("is_team")]
        public bool IsTeam { get; set; }

        [JsonProperty("localModifyTime")]
        public DateTime LocalModifyTime { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("neid")]
        public string Neid { get; set; }

        [JsonProperty("nsid")]
        public long Nsid { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("path_type")]
        public string PathType { get; set; }

        [JsonProperty("pid")]
        public string Pid { get; set; }

        [JsonProperty("prefix_neid")]
        public string PrefixNeid { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("share_to_personal")]
        public bool ShareToPersonal { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("support_preview")]
        public string SupportPreview { get; set; }

        [JsonProperty("updator")]
        public string Updator { get; set; }

        [JsonProperty("updator_uid")]
        public long UpdatorUid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
        #endregion

        public bool success
        {
            get
            {
                return Result == "success";
            }
        }

        public bool IsDetailed = true;
        internal string GetName()
        {
            var tmp = Path.Last()=='/' ? Path.Substring(0, Path.Length - 1) : Path;
            //s_log.Log(LogLevel.Info,()=> tmp.Substring(tmp.LastIndexOf("/") + 1));
            return tmp.Substring(tmp.LastIndexOf("/") + 1);
            //return "tmptmpt";

        }

        public int DirectoryCount { 
            get {
                if (!IsDetailed) return 0;
                return Content.Count(x => x.IsDir);
            } 
        }
        public int FileCount {
            get{
                if (!IsDetailed) return 0;
                return Content.Count(x => !x.IsDir);
            }
        }
        public int ItemCount {
            get{
                if (!IsDetailed) return 0;
                return Content.Length;
            }
        }
        public int VisibleCount {
            get{
                if (!IsDetailed) return 0;
                return Content.Count(x => !x.IsDisplay);
            }
        }
        internal IEnumerable<JboxDirectoryInfo> GetDirectories()
        {
            if (!IsDetailed)
                MergeResults(JboxService.GetJboxItemInfo(Path));
            foreach (var item in Content)
            {
                if (item.IsDir)
                {
                    var tmp = item.ToJboxDirectoryInfo();
                    tmp.IsDetailed = false;
                    yield return tmp;
                }
            }
        }

        internal IEnumerable<JboxFileInfo> GetFiles()
        {
            if (!IsDetailed)
                MergeResults(JboxService.GetJboxItemInfo(Path));
            foreach (var item in Content)
            {
                if (!item.IsDir)
                {
                    var tmp = item.ToJboxFileInfo();
                    tmp.IsDetailed = false;
                    yield return tmp;
                }
            }
        }

        private void MergeResults(JboxItemInfo info)
        {
            if (!info.success)
                return;
            this.Content = info.Content;
            this.IsDetailed = true;
        }
    }

    public class JboxFileInfo
    {
        #region Json Properties
        [JsonProperty("access_mode")]
        public long AccessMode { get; set; }

        [JsonProperty("approveable")]
        public bool Approveable { get; set; }

        [JsonProperty("authable")]
        public bool Authable { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("creator_uid")]
        public long CreatorUid { get; set; }

        [JsonProperty("delivery_code")]
        public string DeliveryCode { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("dir_type")]
        public long DirType { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("from_name")]
        public string FromName { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("is_bookmark")]
        public bool IsBookmark { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("is_dir")]
        public bool IsDir { get; set; }

        [JsonProperty("is_display")]
        public bool IsDisplay { get; set; }

        [JsonProperty("is_group")]
        public bool IsGroup { get; set; }

        [JsonProperty("is_shared")]
        public bool IsShared { get; set; }

        [JsonProperty("is_team")]
        public bool IsTeam { get; set; }

        [JsonProperty("localModifyTime")]
        public DateTime LocalModifyTime { get; set; }

        [JsonProperty("lock_uid")]
        public long LockUid { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("neid")]
        public string Neid { get; set; }

        [JsonProperty("nsid")]
        public long Nsid { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("path_type")]
        public string PathType { get; set; }

        [JsonProperty("pid")]
        public string Pid { get; set; }

        [JsonProperty("prefix_neid")]
        public string PrefixNeid { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("rev")]
        public string Rev { get; set; }

        [JsonProperty("rev_index")]
        public long RevIndex { get; set; }

        [JsonProperty("router")]
        public Dictionary<string, object> Router { get; set; }

        [JsonProperty("share_to_personal")]
        public bool ShareToPersonal { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("support_preview")]
        public string SupportPreview { get; set; }

        [JsonProperty("updator")]
        public string Updator { get; set; }

        [JsonProperty("updator_uid")]
        public long UpdatorUid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
        #endregion
        public bool IsDetailed = true;

        internal string GetName()
        {
            return Path.Substring(Path.LastIndexOf("/") + 1);
        }

        internal Stream OpenRead()
        {
            return JboxService.GetFile(Path, Bytes);
        }

        internal Stream OpenRead(long start, long end)
        {
            return JboxService.GetFile(Path, Bytes, start, end);
        }
    }

    public class JboxItemInfo
    {
        #region Json Properties
        [JsonProperty("access_mode")]
        public long AccessMode { get; set; }

        [JsonProperty("approveable")]
        public bool Approveable { get; set; }

        [JsonProperty("authable")]
        public bool Authable { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        [JsonProperty("content")]
        public JboxItemInfo[] Content { get; set; }

        [JsonProperty("content_size")]
        public long ContentSize { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("creator_uid")]
        public long CreatorUid { get; set; }

        [JsonProperty("delivery_code")]
        public string DeliveryCode { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("dir_type")]
        public long DirType { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("from_name")]
        public string FromName { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("is_bookmark")]
        public bool IsBookmark { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("is_dir")]
        public bool IsDir { get; set; }

        [JsonProperty("is_display")]
        public bool IsDisplay { get; set; }

        [JsonProperty("is_group")]
        public bool IsGroup { get; set; }

        [JsonProperty("is_shared")]
        public bool IsShared { get; set; }

        [JsonProperty("is_team")]
        public bool IsTeam { get; set; }

        [JsonProperty("localModifyTime")]
        public DateTime LocalModifyTime { get; set; }

        [JsonProperty("lock_uid")]
        public long LockUid { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("neid")]
        public string Neid { get; set; }

        [JsonProperty("nsid")]
        public long Nsid { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("path_type")]
        public string PathType { get; set; }

        [JsonProperty("pid")]
        public string Pid { get; set; }

        [JsonProperty("prefix_neid")]
        public string PrefixNeid { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("rev")]
        public string Rev { get; set; }

        [JsonProperty("rev_index")]
        public long RevIndex { get; set; }

        [JsonProperty("router")]
        public Dictionary<string, object> Router { get; set; }

        [JsonProperty("share_to_personal")]
        public bool ShareToPersonal { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("support_preview")]
        public string SupportPreview { get; set; }

        [JsonProperty("updator")]
        public string Updator { get; set; }

        [JsonProperty("updator_uid")]
        public long UpdatorUid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
        #endregion

        public bool success
        {
            get
            {
                return Result == "success";
            }
        }

        public bool IsDetailed = true;

        internal JboxDirectoryInfo ToJboxDirectoryInfo()
        {
            return new JboxDirectoryInfo()
            {
                LocalModifyTime = LocalModifyTime,
                IsTeam = IsTeam,
                Content = Content,
                Path = Path,
                IsDeleted = IsDeleted,
                ContentSize = ContentSize,
                Modified = Modified,
                AccessMode = AccessMode,
                Size = Size,
                Bytes = Bytes,
            };
        }

        internal JboxFileInfo ToJboxFileInfo()
        {
            return new JboxFileInfo()
            {
                LocalModifyTime = LocalModifyTime,
                IsTeam = IsTeam,
                Path = Path,
                IsDeleted = IsDeleted,
                Modified = Modified,
                AccessMode = AccessMode,
                Size = Size,
                Bytes = Bytes,
                LockUid = LockUid,
                MimeType = MimeType,
                Hash = Hash
            };
        }

        internal string GetName()
        {
            if (IsDir)
            {
                var tmp = Path.Last() == '/' ? Path.Substring(0, Path.Length - 1) : Path;
                return tmp.Substring(tmp.LastIndexOf("/") + 1);
            }
            else
                return Path.Substring(Path.LastIndexOf("/") + 1);
        }

        internal Stream OpenRead()
        {
            return JboxService.GetFile(Path, Bytes);
        }

        internal Stream OpenRead(long start, long end)
        {
            return JboxService.GetFile(Path, Bytes, start, end);
        }

        internal IEnumerable<JboxItemInfo> GetDirectories()
        {
            if (!IsDetailed)
                MergeResults(JboxService.GetJboxItemInfo(Path));
            foreach (var item in Content)
            {
                if (item.IsDir)
                {
                    var tmp = item;
                    tmp.IsDetailed = false;
                    yield return tmp;
                }
            }
        }

        internal IEnumerable<JboxItemInfo> GetFiles()
        {
            if (!IsDetailed)
                MergeResults(JboxService.GetJboxItemInfo(Path));
            foreach (var item in Content)
            {
                if (!item.IsDir)
                {
                    var tmp = item;
                    tmp.IsDetailed = false;
                    yield return tmp;
                }
            }
        }

        private void MergeResults(JboxItemInfo info)
        {
            if (!info.success)
                return;
            this.Content = info.Content;
            this.IsDetailed = true;
        }
    }

    public class JboxCreateDirInfo
    {
        #region Json Properties
        [JsonProperty("access_mode")]
        public string AccessMode { get; set; }

        [JsonProperty("authable")]
        public string Authable { get; set; }

        [JsonProperty("bytes")]
        public string Bytes { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("creator_uid")]
        public long CreatorUid { get; set; }

        [JsonProperty("delivery_code")]
        public string DeliveryCode { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("from_name")]
        public string FromName { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("is_dir")]
        public bool IsDir { get; set; }

        [JsonProperty("is_shared")]
        public bool IsShared { get; set; }

        [JsonProperty("is_team")]
        public bool IsTeam { get; set; }

        [JsonProperty("modified")]
        public long Modified { get; set; }

        [JsonProperty("neid")]
        public string Neid { get; set; }

        [JsonProperty("nsid")]
        public long Nsid { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("path_type")]
        public string PathType { get; set; }

        [JsonProperty("pid")]
        public string Pid { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("share_to_personal")]
        public bool ShareToPersonal { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("updator")]
        public string Updator { get; set; }

        [JsonProperty("updator_uid")]
        public long UpdatorUid { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
        #endregion
        public bool success
        {
            get
            {
                return Result == "success";
            }
        }

        internal JboxDirectoryInfo ToJboxDirectoryInfo()
        {
            return new JboxDirectoryInfo()
            {
                IsTeam = IsTeam,
                Path = Path,
                IsDeleted = IsDeleted,
                Modified = new DateTime(1970, 1, 1, 0, 0, 0, 0) + TimeSpan.FromMilliseconds(Modified),
            };
        }
    }

    public class JboxMoveItemInfo
    {
        #region Json Properties
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
        #endregion
        public bool success
        {
            get
            {
                return Result == "success";
            }
        }
    }

    public class JboxDeliveryAuthDto
    {
        public string token { get; set; }
    }

    public class JboxDeliveryTransferDto
    {
        public string result { get; set; }
        public string type { get; set; }
    }
}
