module.exports = {
    title: 'JboxWebdav 说明文档',
    description: 'Just playing around',
    markdown: {
        lineNumbers: true
    },
    themeConfig: {
        nav: [
            { text: '开始', link: '/start/' },
            {
                text: '下载安装',
                items: [
                    { text: 'Windows', link: '/setup/Windows' },
                    { text: 'macOS', link: '/setup/macOS' },
                    { text: 'Linux', link: '/setup/Linux' },
                    { text: 'Android', link: '/setup/Android' },
                    { text: 'iOS', link: '/setup/iOS' },
                    { text: 'Other', link: '/setup/Other' },
                ]
            },
            { text: '使用', link: '/tip/' },
            { text: '关于', link: '/about/' },
            { text: 'Github', link: 'https://github.com/1357310795/JboxWebdav', target: "_blank" },
        ],
        sidebar: {
            '/start/': getStartSidebar(),
            '/setup/': getSetupSidebar(),
            '/tip/': getTipSidebar(),
            '/about/': [],
        },
        displayAllHeaders: false
    },
    base: "/JboxWebdav/"
};

function getStartSidebar() {
    return [{
        title: '首页',
        collapsable: false,
        children: [
            '',
        ]
    }, ]
};

function getSetupSidebar() {
    return [{
            title: '下载安装主程序',
            collapsable: false,
            children: [
                'Windows',
                'macOS',
                'Linux',
                'Android',
                'iOS',
                'Other',
            ]
        },
        {
            title: '映射为磁盘',
            collapsable: false,
            children: [
                'Mount-Raidrive',
                'Mount-Rclone',
                'Mount-System',
                'Mount-Android',
            ]
        },
    ]
};

function getTipSidebar() {
    return [{
        title: '使用指南',
        collapsable: false,
        children: [
            '',
            'JboxShared',
            'JboxPublic',
            'Config',
            'AutoStart'
        ]
    }, ]
};