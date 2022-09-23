---
title: 使用 YAML 配置文件
lang: zh-CN
---

::: tip 说明
此功能只对控制台应用有效
:::

## 使用 YAML 配置文件初始化设置
1. 新建一个 `.yml` 文件，输入以下内容，并调整设置。

```yaml
# 监听地址，默认http://127.0.0.1:65472/
# 局域网监听地址：http://+:65472/
address: http://127.0.0.1:65472/

# 是否启用“交大空间”
isPublicEnabled: true

# 是否启用“他人的分享链接”
isSharedEnabled: false

# 访问模式：
# Full 完全模式
# ReadOnly 只读模式
# ReadWrite 读写模式
# NoDelete 防止删除模式
accessMode: Full
```

2. 使用参数启动程序
```bash
./JboxWebdav.ConsoleApp -c config.yml
```

3. 看到“配置文件读取成功！”字样则成功，否则请按提示检查。