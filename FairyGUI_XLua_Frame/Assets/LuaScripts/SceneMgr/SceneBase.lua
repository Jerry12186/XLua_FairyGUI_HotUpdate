--场景的基类
SceneBase = {}

function SceneBase:New(o)
    o = o or {}
    setmetatable(o,{__index = self})
    return o
end
--获取场景的名称
function SceneBase.GetSceneName()
    
end

--场景加载完毕
--在这里对场景的东西进行初始化
function SceneBase:OnSceneLoadOver()
    ----生成一个物体，用来关联场景
    --local GameObject = CS.UnityEngine.GameObject
    --local go = GameObject("我是这个场景的老大")
end

--当场景被回收的时候，处理一些事情
function SceneBase:OnSceneUnload()
    
end