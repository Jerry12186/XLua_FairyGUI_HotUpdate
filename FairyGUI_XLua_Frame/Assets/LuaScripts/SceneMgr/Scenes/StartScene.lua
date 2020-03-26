--游戏开始界面
require 'SceneMgr/SceneBase'
require 'UI/Core/UIWindowMgr'

StartScene = SceneBase:New()

function StartScene.GetSceneName()
    return "StartScene"
end

function StartScene:OnSceneLoadOver()
    print("开始游戏界面")
    
    require 'UI/LoginWin'
    UIWindowMgr:OpenWindow("LoginModule",LoginWin)
    require 'Modules/LoginModule'
    LoginModule:InitNet()
end