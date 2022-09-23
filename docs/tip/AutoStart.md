---
title: 配置开机启动（Linux）
lang: zh-CN
---

## Linux 使用 systemd user 配置开机启动
1. 在 systemd 用户目录 `/home/xxx/.config/systemd/user/` 中创建 `jboxwebdav.service` 文件：
```jboxwebdav.service
[Unit]
Description = WebDAV Sercive for jBox
After=network-online.target
Wants=network-online.target systemd-networkd-wait-online.service

[Service]
Type = simple
ExecStart = sh /home/xxx/.config/systemd/user/jbox-startup.sh

[Install]
WantedBy = default.target
```
2. 在 systemd 用户目录：`/home/xxx/.config/systemd/user/` 中创建启动脚本 `jbox-startup.sh`：
```bash
cd /path_to_your_location/JboxWebdav.ConsoleApp-Linux-x64 &&  ./JboxWebdav.ConsoleApp  -no-interactive  -c config.yml
```
其中  
`-no-interactive` 必选，非交互模式，否则会导致报错无法启动
` -c config.yml` 可选，自定义配置，详见 [使用 YAML 配置文件初始化设置](Config.md)

3. 启动服务/设置开机启动

```bash
# 重载守护进程，读取 jbox-startup.service 文件
systemctl --user daemon-reload
# 启动服务状态
systemctl --user start jboxwebdav
# 查看服务状态，可以此命令查看报错日志
systemctl --user status jboxwebdav
# 开机启动 jboxwebdav 服务
systemctl --user enable jboxwebdav
```


