---
title: 关于本项目
lang: zh-CN
---

## Q&A

### Q1：程序要求输入我的 jAccount 账号密码，是否安全？我的认证信息被保存在服务器吗？
jBox 的访问涉及到 jAccount 认证，为方便起见，我逆向了 jAccount 的 Oauth2 认证过程，只需用户在程序内输入用户名和密码即可登录（感谢 [SJTU-PLUS提供的验证码识别服务](https://github.com/PhotonQuantum/jaccount-captcha-solver)）。程序不会将您的用户名和密码用于除登录以外的用途。（若不放心，可审查源代码后自行编译）

JboxWebdav 没有服务器，一切操作都在用户端完成。用户的认证信息经过加密后被保存在本地。

### Q2：我的文件安全吗？会不会被误删？
JboxWebdav 没有服务器，不可能上传您的文件。和本地磁盘一样，映射的 WebDAV 存储能被任何程序读取、操作。如果怕文件被误删，可以在程序设置里开启“防止删除模式”。

### Q3：我可以在树莓派上访问交大云盘吗？Android 上运行 JboxWebdav 总是被杀怎么办？
得益于 WebDAV 是基于 http 的应用，理论上任何支持 http 协议的设备都能使用 WebDAV 进行文件传输。您可以在任何受支持的系统上运行 JboxWebdav，更改监听地址为设备的对外 ip 地址（注意检查程序是否具有权限），然后，在不受支持的系统上通过此 ip 地址来访问 WebDAV 资源。

### Q4：是否提供 32 位 Windows 可执行文件？
暂时不提供，如有需要，可以 fork 一份仓库，改一下 Github Action 的配置文件，让 Github Action 帮你编译一份 32 位程序。或者提起 issue。
