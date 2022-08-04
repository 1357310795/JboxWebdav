---
title: Windows
lang: zh-CN
---

# 在 Windows 系统下安装

## 可用的应用程序类型
JboxWebdav 提供多种应用程序框架，以达到跨平台支持。以下是在 Windows 系统下可用的应用程序框架：

- [**WPF 框架（推荐）**](#使用-wpf-框架)
- [MAUI 框架](#使用-maui-框架)
- [控制台应用](#使用控制台应用)

## 使用 WPF 框架
### 安装步骤
1. [点击这里](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0/runtime)下载安装.NET 6.0运行时框架。点击 **“运行桌面应用”** 下的“下载x64”按钮以下载安装包。下载完成后运行，根据向导进行安装。

::: tip 提示
如果你知道什么是.NET 6.0运行时，并且明确计算机中已经安装，可以跳过这一步。
:::

2. 在[Releases](https://github.com/1357310795/JboxWebdav/releases)下载 JboxWebdav 的可执行文件（请选择以 **WpfApp-Windows** 结尾的项目）。
3. 直接运行程序。
4. 若未处于交大内网环境，程序会提示连接交大 VPN，连接好后继续。

![](https://s2.loli.net/2022/08/01/7bi1gGhDj2EIwXm.png)

5. 登录 Jaccount（安全性问题见[这里](../about/readme.md#安全性说明)）

![](https://s2.loli.net/2022/07/03/YXpRmdWC1QHSMrz.png)

6. 直接点击“启动”按钮运行 Webdav 服务。

![](https://s2.loli.net/2022/08/01/YujkUPdKIiXfbAT.png)

7. 最小化程序，可以让程序在后台运行。

8. 接下来请阅读[验证可用性&下一步](#验证可用性-下一步)

## 使用 MAUI 框架
### 安装步骤
1. 在[Releases](https://github.com/1357310795/JboxWebdav/releases)下载 JboxWebdav 的可执行文件（请选择以 **MauiApp-Windows** 结尾的项目）。
2. 解压压缩包

![](https://s2.loli.net/2022/08/01/a6TCM1QZJBI8sNR.png)

3. 在`Install.ps1`上右键，点击`使用 PowerShell 运行`，根据提示安装证书和应用。
4. 在开始菜单找到 JboxWebdav，点击启动应用。
4. 若未处于交大内网环境，程序会提示连接交大 VPN，连接好后继续。

![](https://s2.loli.net/2022/08/01/KjLmOkc2FSVM94s.png)

5. 登录Jaccount（安全性问题见[这里](../about/readme.md#安全性说明)）

![](https://s2.loli.net/2022/08/01/q9zWCLYDGoHAPMt.png)

6. 直接点击“运行”按钮运行Webdav服务。

![](https://s2.loli.net/2022/08/01/LTnyPaBmrdpOjHR.png)

7. 接下来请阅读[验证可用性&下一步](#验证可用性-下一步)

## 使用控制台应用
### 安装步骤
1. [点击这里](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0/runtime)下载安装.NET 6.0运行时框架。点击 **“运行控制台应用”** 下的“下载x64”按钮以下载安装包。下载完成后运行，根据向导进行安装。

::: tip 提示
如果你知道什么是.NET 6.0运行时，并且明确计算机中已经安装，可以跳过这一步。
:::

2. 在[Releases](https://github.com/1357310795/JboxWebdav/releases)下载JboxWebdav的可执行文件（请选择以 **ConsoleApp-Windows** 结尾的项目）。
3. 直接运行程序。
4. 根据提示登录后，服务自动运行。

![](https://s2.loli.net/2022/08/01/h9mkcH5ta3nPpSG.png)

5. 接下来请阅读[验证可用性&下一步](#验证可用性-下一步)

## 验证可用性&下一步
打开浏览器访问您在 JboxWebdav 里设置的监听地址（默认为 [http://127.0.0.1:65472/](http://127.0.0.1:65472/) ），若看到`JboxWebdav Server Running!`则表示服务运行成功。

接下来请参阅[映射为磁盘](../setup/Mount-Raidrive.md)章节
