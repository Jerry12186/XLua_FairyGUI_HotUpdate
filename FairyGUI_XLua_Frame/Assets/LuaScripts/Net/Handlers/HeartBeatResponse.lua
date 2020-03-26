require 'Net/ResponseBase'
require 'Net/HandleResponse'

HeartBeatResponse = ResponseBase:new()

function HeartBeatResponse:HandleMsg(msg)
    print("心跳回复---->"..msg.MsgId)
end

HandleResponse.CmdList[2] = HeartBeatResponse