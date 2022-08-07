---
title: 使用系统功能
lang: zh-CN
---

# 使用系统功能挂载 WebDAV 服务器

::: warning 警告
不推荐此种方式！
:::

## Windows
### 使用方法
Windows 系统资源管理器允许映射 WebDAV 服务器，步骤如下：
1. 设置允许连接到 http 服务器

打开注册表，找到`HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WebClient\Parameters`，把`BasicAuthLevel`的值改为`2`

2. 解除50MB文件大小限制

打开注册表，找到`HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WebClient\Parameters`，把`FileSizeLimitInBytes`的值改为`4294967295`（十进制）

3. 映射 WebDAV 服务器

![](https://s2.loli.net/2022/08/02/FDjeWiUIxGlRV8b.png)

### 兼容性说明
- Windows 的映射驱动器功能不支持部分读取，视频可能无法播放，大文件操作可能会卡死资源管理器
- 映射驱动器后，若 WebDAV Server 无响应，则 Windows 资源管理器也会卡死

## Linux
### 使用方法
以 KDE 桌面环境的 Dolphin 为例，点击“添加网络文件夹”，按照向导操作即可。
![](https://s2.loli.net/2022/08/02/ojs3mUX1ZGrSPCx.png)
![](https://s2.loli.net/2022/08/02/654fU1z3hIAtwvn.png)

## macOS
::: danger 警告
请不要使用访达的连接到服务器功能！macOS 会对每个文件夹读取对应的`.DS_Store`，严重影响体验。
:::
