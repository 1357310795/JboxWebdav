---
title: 使用 Rclone
lang: zh-CN
---

# 使用 Rclone 挂载 Webdav 服务器

::: warning 注意
Rclone 仅支持桌面端操作系统（Windows、Linux、macOS）
:::
::: danger 警告
Rclone 与 JboxWebdav 存在部分兼容性问题，详见[**此处**](#兼容性说明)
:::

## 安装步骤
1. 在 [Rclone 官方 Releases](https://github.com/rclone/rclone/releases) 下载对应系统版本和位数的程序包。
2. 安装或解压 Rclone。
3. 打开命令行/终端，输入命令创建服务器配置。
```bash
# 以 Windows 为例
rclone config create jbox webdav url=http://127.0.0.1:65472/ vendor=other --non-interactive
```
4. 在命令行/终端使用命令启动 Rclone 挂载
```bash
# Windows 可使用网络磁盘模式
rclone mount jbox: Y: --network-mode --volname \\server\jbox --vfs-cache-mode=minimal
# Linux 需要映射到一个空文件夹
rclone mount jbox: /home/jbox --vfs-cache-mode=minimal
```

::: tip 提示
关于 Rclone 挂载的更多选项请参阅 [Rclone 官方文档](https://rclone.org/commands/rclone_mount/)
:::

接下来，或许您还想阅读[使用](../tip/)章节

## 兼容性说明
- 由于 Rclone 的缓存机制，将文件从外部复制到 jBox 时，进度条会直接到 100%，等文件上传完成才会结束。故不推荐上传大文件。
- 由于 Rclone 的缓存机制，[“他人的分享链接”功能](../tip/JboxShared.md)体验不佳，可能需要重新挂载才能看到分享文件。
