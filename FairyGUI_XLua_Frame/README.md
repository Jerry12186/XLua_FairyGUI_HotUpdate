# FairyGUI_XLua_Frame1

#### 介绍
本段代码是在Unity3D中使用xLua+FairyGUI编写的一套还算完整的前端框架。
其中包括热更新、网络通信。

#### 软件架构
Unity版本：2018.4.14.f1

xLua信息：
    1. Lua  5.1.5 & 5.3.3 & 5.3.4
       Copyright (C) 1994-2016 Lua.org, PUC-Rio.
    2. LuaJIT  2.1.0beta2 
       Copyright (C) 2005-2016 Mike Pall. All rights reserved.
    3. luasocket  3.0-rc1
       Copyright ? 1999-2013 Diego Nehab. All rights reserved.
    4. Cecil 0.9.6
       Copyright (c) 2008 - 2015 Jb Evain
       Copyright (c) 2008 - 2011 Novell, Inc.
    其中xlua中融合Json、pb等lua库。

FairyGUI版本：5.0.6

#### 安装教程
0.  下载之后，把Plugins下的iOS1文件夹改成iOS。
1.  配置热更新服务器，就是一个web服务器。服务器地址配置在UpdateConfig.cs中。
2.  使用Unity3D菜单栏上的“工具->打包AB for iOS(Android)”，打包出的文件夹在项目根目录下，文件夹名字是iOS(或Android)。
3.  把打包出的文件夹放到热更新服务器地址下。
4.  打开Resources->Scenes下的InitScene场景，运行即可。
5.  网络测试，则需要运行项目根目录下的TcpServer.js(这是用nodejs编写的，需要配置nodejs环境)。

### Android打包
1.  如果热更新服务器是本地的，确保跟手机在同一局域网下。
2.  删除Resources文件夹下所有文件，保留InitScene。
3.  打包即可。

### 后续计划
    由于苹果提审过程中，不能从网络下载资源，需要搞一个压缩包，把ab文件通过解压的方式放到Application.persistentDataPath下。
    所有，后面会加入压缩和解压的功能。
    
#### 参与贡献
    By Jerry
    