![](https://s2.loli.net/2022/07/03/RrHhN7guEfbn8lI.png)

## 主要功能
程序对接了Jbox Api和Webdav协议，用户可以通过Webdav协议访问Jbox，借助Rclone可将Jbox挂载为网络磁盘，使用体验接近本地磁盘。

![Topology.png](https://s2.loli.net/2022/07/03/lXygYmUTZKpaVd7.png)
![intro-p1.png](https://s2.loli.net/2022/07/03/nQGUeVpHfKYWX92.png)
![intro-p2.png](https://s2.loli.net/2022/07/03/D1oG4VvMLRKTzbp.png)
![intro-p3.png](https://s2.loli.net/2022/07/03/bivVemC479G2rsS.png)

## 使用建议
推荐用于： 
- 在线观看网盘内的电影（加载速度非常快，支持播放、回放）
- 下载大文件（接近满速）
- 在线打开编辑Word/Excel/PowerPoint文件

不推荐用于：
- 整理文件（由于Webdav限制，不支持批量移动、删除、重命名操作，故速度较慢）
- 查看图库预览（预览图片没法用jbox提供的预览接口，而是需要读取整个文件，速度较慢）
- 解压压缩包（由于Rclone存在写入缓存，在缓存全部上传到jbox后才会正常显示解压的文件）
- 下载小文件（下载小文件需要一个一个获取，效率极低，不如去jbox用批量下载）
- 上传大文件（由于Rclone的奇妙缓存机制，上传文件请不要超过100MB）
- 作为Git存储库（超级多的小文件是灾难！）

## 下载安装
1. [点击下载](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/sdk-6.0.301-windows-x86-installer)安装.NET 6.0运行时框架（如果你不知道.NET 6.0是什么）。
2. 在[Releases](https://github.com/1357310795/JboxWebdav/releases)下载程序的可执行文件。
3. 解压、运行程序。
4. 首次运行，需要安装WinFsp、配置Rclone，完成后即可打开程序。

![](https://s2.loli.net/2022/07/03/a2JxGDCe9imPUtX.png)

5. 登录Jaccount

![](https://s2.loli.net/2022/07/03/YXpRmdWC1QHSMrz.png)

6. 直接点击“运行”按钮运行Webdav服务，然后运行Rclone Mount。

![](https://s2.loli.net/2022/07/03/jNS92TGnDsZ758o.png)

7. 点击最小化程序，可以让程序在后台运行。

## 安全性说明
Jbox的访问涉及到Jaccount认证，为方便起见，我逆向了Jaccount的Oauth2认证过程，只需用户在程序内输入用户名和密码即可登录（感谢SJTU-PLUS提供的验证码识别服务）。程序不会将您的用户名和密码用于除登录以外的用途。（若不放心，可审查源代码后自行编译）

## 跨平台支持
软件主体使用.NET 6.0开发，支持跨平台，图形界面应用JboxWebdav.WpfApp使用WPF框架，仅支持Windows平台；控制台应用JboxWebdav.ConsoleApp支持Windows、MacOS、Linux，如有需要可自行编译。Rclone是跨平台应用，mount功能在MacOS、Linux系统上均可用，使用方法请自行查阅官方文档。

## 说在最后
如果觉得程序好用的话，求一个小小的Star

当然也欢迎Bug Report & Feature Request
