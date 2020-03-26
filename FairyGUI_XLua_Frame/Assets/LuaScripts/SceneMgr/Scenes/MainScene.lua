--游戏主城场景
require 'SceneMgr/SceneBase'
require 'UI/Core/Timer'
require 'ResMgr/ResMgr'

MainScene = SceneBase:New()

function MainScene.GetSceneName()
	return "MainScene"
end

--场景加载完毕需要做的时间
function MainScene:OnSceneLoadOver()
	function OnCutDown(obj)
		ResMgr:Update()
	end
	self.CutDown = OnCutDown
	--1s清空一下资源引用
    Timer:AddTimer(1,0,self.CutDown,obj)
end