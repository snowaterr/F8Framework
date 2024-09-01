zim-1.01：实现一键添加LogViewer：在开发工具菜单中添加按钮“添加LogViewer至当前场景”，同时丰富了Log模块的唤醒方式

zim-1.02：修改LogF8.LogWarning，将打印出黄色log标题

zim-1.03：增加场景管理模块，支持根据build index加载场景，支持同步、异步（是否平滑加载）2种模式加载场景，支持注册委托函数（场景加载和卸载）

zim-1.04：UI增加一些小功能，可以从BaseView类快速获取RectTransform组件和该UI所属的图层（canvas），UIManager.Initialize方法增加参数，用于修改UI的渲染模式和缩放模式，并且默认缩放模式改为CanvasScaler.ScaleMode.ScaleWithScreenSize

zim-1.05: 为ZIM自定义模块添加初始化样例和FF8全局引用。修改UI管理模块，UIManager还会设置eventsystem为dontdestroy