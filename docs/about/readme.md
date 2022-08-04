---
title: 关于本项目
lang: zh-CN
---

## Q&A

### Q1：程序要求输入我的 Jaccount 账号密码，是否安全？我的认证信息被保存在服务器吗？
Jbox 的访问涉及到 Jaccount 认证，为方便起见，我逆向了 Jaccount 的 Oauth2 认证过程，只需用户在程序内输入用户名和密码即可登录（感谢 [SJTU-PLUS提供的验证码识别服务](https://github.com/PhotonQuantum/jaccount-captcha-solver)）。程序不会将您的用户名和密码用于除登录以外的用途。（若不放心，可审查源代码后自行编译）

JboxWebdav 没有服务器，一切操作都在用户端完成。用户的认证信息经过加密后被保存在本地。

### Q2：我的文件安全吗？会不会被误删？
JboxWebdav 没有服务器，不可能上传您的文件。和本地磁盘一样，映射的 Webdav 存储能被任何程序读取、操作。如果怕文件被误删，可以在程序设置里开启“防止删除模式”。