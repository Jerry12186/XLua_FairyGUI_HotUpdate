--TCP连接相关
require 'Net/HandleResponse'
require 'Tools/Event'

TcpHelper = {}
local Tcp = CS.NetModel.NetHelper
TcpHelper.netDelay = 0
local oldTime = 0
local heartBeatLose = 0--心跳次数检测

--同步直接注册心跳没有问题
function TcpHelper:ConnectBySync(ip,port)
    local isOk = Tcp.CreateConnectBySync(ip,port)
    if isOk then
        print("同步连接网络成功")
        Tcp.RegisterHeartBeat(TcpHelper.HeartBeat,3)
    end
    return isOk
end

--异步连接心跳注册不了
--已经搞定
function TcpHelper:ConnectByAsync(ip,port)
    function Callback(isOk)
        if isOk then
            print("异步连接服务器成功")
            Tcp.RegisterHeartBeat(TcpHelper.HeartBeat,3)
        else
            print("服务器连接失败o(╥﹏╥)o")
        end
    end
    Tcp.CreateConnectByAsync(ip,port,Callback)
end

--关闭网络连接
function TcpHelper:CloseConnect()
    Tcp.CloseConnect()
end

--这个需要修改一下
--已经改好
function TcpHelper:SendMsg(msgId,msgBody)
    local pb_data = nil
    if msgBody ~= nil then
        pb_data = msgBody
    end

    if pb_data == nil then
        Tcp.SendMessage(msgId)
    else
        Tcp.SendMessage(msgId,pb_data)
    end
    print("发送消息--->"..msgId)
end

--处理接收的消息
function TcpHelper.OnReciveMsg(msg)
    if(msg.MsgId == 2) then --这里的MsgId是规定好的服务器返回的心跳消息id
        TcpHelper.netDelay = Tcp.GetSystemTime() - oldTime
        oldTime = Tcp.GetSystemTime()
        print("网络延迟----->"..TcpHelper.netDelay.."ms")
    end
    print("------->"..msg.MsgId)
    heartBeatLose = 0--接收到消息后，就把失去心跳次数改成0
    HandleResponse:ExcuteHandle(msg)
end
Tcp.RegisterLuaMsgHandler(TcpHelper.OnReciveMsg)

--断线事件
function TcpHelper.OnNetEx()
    print("啊，我断线了")
    Event:Broadcast("OffLine")
    --这里告诉全世界我掉线了
end
Tcp.RegisterNetExEvent(TcpHelper.OnNetEx)

function TcpHelper.HeartBeat()
    --失去心跳次数大于一定的数，就断开连接
    if heartBeatLose >= 3 then
        print("收不到服务器的心跳回复，服务端接锅🤣🤣🤣")
        Tcp:CloseConnect()
        heartBeatLose = 0
    end
    TcpHelper:SendMsg(1)
    --每发送一次心跳，心跳次数就+1
    heartBeatLose = heartBeatLose + 1
    oldTime = Tcp.GetSystemTime()
end