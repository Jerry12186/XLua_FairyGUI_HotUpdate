require 'FairyGUI'
require 'UI/Core/UIWindowMgr'
require 'SceneMgr/SceneMgr'

--注册处理消息
--require 'Net/Handlers/LoginResponse'
--测试心跳
require 'Net/Handlers/HeartBeatResponse'

xutil = require 'xlua.util'
--加载ProtocolBuff
require 'Net/ProtoBuff/ProtoLoad'
ProtoLoad:loadfile("ProTest")

Debug.log('lua启动成功')

require 'SceneMgr/Scenes/StartScene'
SceneMgr:LoadScene(StartScene)

--UIWindowMgr:OpenWindow("StartUI",LoginWin)
-- local BaseClass = CS.Tutorial.BaseClass
-- BaseClass.BSFunc()
-- local test = BaseClass()
-- test:BMFunc()

-- local ResMgr = CS.MyABSys.ResMgr
-- Debug.log(ResMgr.Instance:LoadUI("3333",nil))