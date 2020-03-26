-- 加载游戏物体用的
-- 其实直接用CS.MyABSys.ResMgr也行，只不过在封装一层方便而已
local ResHelper = CS.MyABSys.ResHelper

ResMgr = {}

function ResMgr:Update()
	ResHelper.Instance:Update()
end

--异步加载资源
function ResMgr:LoadByAsync(resName,callback,bPublic)
	if bPublic == nil then
		bPublic = false
	end
	local gameObject = nil
	function OnLoadOver(obj)
		if CS.UnityEngine.Application.isEditor then
			gameObject = obj
		else
			local res = CS.System.IO.Path.GetFileName(resName)
			gameObject = obj:LoadAsset(res)
		end
		callback(gameObject)
	end
	ResHelper.Instance:LoadResFromABAsync(resName,OnLoadOver,bPublic)
end

--同步加载资源，这个会造成界面卡顿，尽量少用
function ResMgr:LoadBySyn(resName,bPublic)
	if bPublic == nil then
		bPublic = false
	end
	local object = ResHelper.Instance:LoadResFromABSync(resName,bPublic)
	return object
end