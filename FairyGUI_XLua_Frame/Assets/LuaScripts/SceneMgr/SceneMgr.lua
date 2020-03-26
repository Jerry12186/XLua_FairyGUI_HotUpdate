--场景管理器
SceneMgr = {}
--require 'UI/LoadingWin'
require 'UI/Core/UIWindowMgr'
require 'Tools/Timer'
local SceneHelper = CS.MyScene.SceneMgr
local ResHelper = CS.MyABSys.ResHelper

SceneMgr.currentScene = nil

--参数表示要加载的场景
--即SceneBase派生的类
function SceneMgr:LoadScene(scene)
    print("开始加载场景--->"..scene.GetSceneName())
    self.currentScene = scene
    --LoadingWin:SetProgress(0)
    
    --场景加载完成
    function OnSceneLoadOver(sceneName)

        print("场景--->"..sceneName.."---加载完毕")
        --销毁上一个场景的所有UI，因为FGUI的UI是一直都留在内存里的
        UIWindowMgr:DisposeAllWindows()
        
        self.currentScene:OnSceneLoadOver()
    end
    --更新进度条
    function UpdateProgress(progress)
        -- print("场景加载进度------>"..progress)
    end
    if  CS.UnityEngine.Application.isEditor then
        print("Editor中的场景")
        SceneHelper.LoadSceneAsync(SceneMgr.currentScene.GetSceneName(),UpdateProgress,OnSceneLoadOver)
    else
        print("AB中的场景")
        function OnABLoadOver(obj)
            print(obj.name)
            SceneHelper.LoadSceneAsync(SceneMgr.currentScene.GetSceneName(),UpdateProgress,OnSceneLoadOver)
        end
        ResHelper.Instance:LoadResFromABAsync("Scenes/"..SceneMgr.currentScene.GetSceneName(),OnABLoadOver,true)
    end
end