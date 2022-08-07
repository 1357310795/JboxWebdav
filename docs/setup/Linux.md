---
title: Linux
lang: zh-CN
---

# 在 Linux 系统下安装

## 可用的应用程序类型
JboxWebdav 提供多种应用程序框架，以达到跨平台支持。以下是在 Linux 系统下可用的应用程序框架：

- [控制台应用](#使用控制台应用)

## 使用控制台应用
### 安装步骤
1. 根据[微软官方文档](https://docs.microsoft.com/zh-cn/dotnet/core/install/linux)安装.NET 6.0 Runtime

::: tip 提示
只需安装 Runtime，无需安装 SDK
:::

省流：
```bash
# Ubuntu
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-runtime-6.0
```

```bash
# CentOS 7
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum install dotnet-runtime-6.0
```

```bash
# Debian
wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get install -y dotnet-runtime-6.0
```

```bash
# Fedora
sudo dnf install dotnet-runtime-5.0
```

2. 在[Releases](https://github.com/1357310795/JboxWebdav/releases)下载 JboxWebdav 的可执行文件（请选择包含 **ConsoleApp-Linux** 的条目）。
3. 解压，在文件夹内启动终端
4. 使用命令`JboxWebdav.ConsoleApp`运行程序。
5. 根据提示登录后，服务自动运行。

![](https://s2.loli.net/2022/08/02/rKv9aQCSIknRbJu.png)

6. 接下来请阅读[验证可用性&下一步](#验证可用性-下一步)

## 验证可用性&下一步
打开浏览器访问您在 JboxWebdav 里设置的监听地址（默认为 [http://127.0.0.1:65472/](http://127.0.0.1:65472/) ），若看到`JboxWebdav Server Running!`则表示服务运行成功。

接下来请参阅[映射为磁盘](../setup/Mount-Rclone.md)章节