---
title: macOS
lang: zh-CN
---

# 在 macOS 系统下安装

## 可用的应用程序类型
JboxWebdav 提供多种应用程序框架，以达到跨平台支持。以下是在 macOS 系统下可用的应用程序框架：

- [**控制台应用（推荐）**](#使用控制台应用)
- [MAUI 框架](#使用-maui-框架)

## 使用控制台应用
### 安装步骤
1. [点击这里](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0/runtime)下载安装.NET 6.0运行时框架。先切换到“macOS”选项卡，点击“运行应用”下的“下载x64”按钮以下载安装包。下载完成后运行，根据向导进行安装。

2. 在[Releases](https://github.com/1357310795/JboxWebdav/releases)下载JboxWebdav的可执行文件（请选择以 **ConsoleApp-macOS** 结尾的项目）。
3. 解压缩。
4. 在文件夹内运行终端，输入命令`./JboxWebdav.ConsoleApp`运行程序。
5. 根据提示登录后，服务自动运行。

![](https://s2.loli.net/2022/08/01/q3hUiFaKIS9ZEc5.png)

6. 接下来请阅读[验证可用性&下一步](#验证可用性-下一步)

## 使用 MAUI 框架
### 安装步骤
1. 在[Releases](https://github.com/1357310795/JboxWebdav/releases)下载JboxWebdav的可执行文件（请选择以 **MauiApp-macOS** 结尾的项目）。
2. 双击解压缩。
3. 使用“右键——打开”运行程序
4. 根据提示登录后，点击“运行”按钮运行Webdav服务。
5. 接下来请阅读[验证可用性&下一步](#验证可用性-下一步)

## 验证可用性&下一步
打开浏览器访问您在 JboxWebdav 里设置的监听地址（默认为 [http://127.0.0.1:65472/](http://127.0.0.1:65472/) ），若看到`JboxWebdav Server Running!`则表示服务运行成功。

接下来请参阅[映射为磁盘](../setup/Mount-Rclone.md)章节