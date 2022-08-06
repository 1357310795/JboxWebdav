---
title: Android
lang: zh-CN
---

# 在 Android 系统下安装

::: warning 注意
由于 JboxWebdav 使用了`In-memory LRU Cache`确保读取文件的性能，对计算机性能要求较高，且需要常驻后台，故不推荐在移动端使用本软件。
:::

## 可用的应用程序类型
JboxWebdav 提供多种应用程序框架，以达到跨平台支持。以下是在 Android 系统下可用的应用程序框架：

- [MAUI 框架](#使用-maui-框架)

## 使用 MAUI 框架
### 安装步骤
1. 在[Releases](https://github.com/1357310795/JboxWebdav/releases)下载 JboxWebdav 的安装包文件（请选择包含 **MauiApp-Android** 的条目）。
2. 使用 Android 程序包管理器安装（若提示类似“未知来源应用”，按提示操作即可）。
3. 启动应用。
4. 根据提示登录后，点击“启动”按钮运行Webdav服务。

![](https://s2.loli.net/2022/08/01/6qBoDxbUcNAaur1.jpg)

5. 接下来请阅读[验证可用性&下一步](#验证可用性-下一步)

## 验证可用性&下一步
打开浏览器访问您在 JboxWebdav 里设置的监听地址（默认为 [http://127.0.0.1:65472/](http://127.0.0.1:65472/) ），若看到`JboxWebdav Server Running!`则表示服务运行成功。

接下来请参阅[Android 访问 Webdav 服务器](../setup/Mount-Android.md)章节