--处理消息回应请求
require 'Config/NetConfig'

HandleResponse = {}
HandleResponse.CmdList = {}

function HandleResponse:ExcuteHandle(msg)
	Handler = HandleResponse.CmdList[msg.MsgId]
	if Handler == nil then
		print("兄嘚，你这个MsgId---->"..msg.MsgId.."  木得处理对象啊")
	else
		Handler:HandleMsg(msg)
	end
end