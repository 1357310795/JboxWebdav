---
title: 配置开机自启动
lang: zh-CN
---

## Linux 使用 systemd user 配置开机自启动
在 systemd 用户目录：`～/.config/systemd/user/` 中创建 `jboxwebdav.service` 文件：
```jboxwebdav.service
[Unit]
Description = Web Dav sercive jbox
After=network-online.target
Wants=network-online.target systemd-networkd-wait-online.service
[Service]
Type = simple
ExecStart = sh /home/xxx/.config/systemd/user/jbox-startup.sh

[Install]
WantedBy = default.target
```
在 systemd 用户目录：`～/.config/systemd/user/` 中创建启动脚本 `jbox-startup.sh`：
```bash
cd /path_to_your_location/JboxWebdav.ConsoleApp-Linux-x64 &&  ./JboxWebdav.ConsoleApp  -no-interactive  -c config.yml
```
### 脚本参数说明：  
`-no-interactive` 必选，非交互模式，否则会导致报错无法启动 

` -c config.yml` 可选，自定义配置，详见 [使用 YAML 配置文件初始化设置](./Config.md)

### 服务启动命令

```bash
# 重载守护进程，读取 jbox-startup.service 文件
systemctl --user daemon-reload
# 查看服务状态，可以此命令查看报错日志
systemctl --user status jboxwebdav
# 开机启动 jboxwebdav
systemctl --user enable jboxwebdav
```


