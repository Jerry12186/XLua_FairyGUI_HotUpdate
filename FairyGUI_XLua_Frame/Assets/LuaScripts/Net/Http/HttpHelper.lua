--Http请求相关

HttpHelper = {}
local http = CS.NetModel.HttpMgr
local form = CS.UnityEngine.WWWForm()

function HttpHelper:Get(url,callback)
	http.HttpGet(url,callback)
end

function HttpHelper:Post(url,callback)
	http.HttpPost(url,form,callback)
end

function HttpHelper:AddField(param1,param2)
	form:AddField(param1,param2)
end