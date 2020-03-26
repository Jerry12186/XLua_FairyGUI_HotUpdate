--事件处理相关

Event = {}

local MyEvent = CS.EventMgr.MyEvent()

--添加事件以及事件处理方法
function Event:AddEvent(eventName,action)
    MyEvent:AddListener(eventName,action)
end

function Event:RemoveEvent(eventName,action)
    MyEvent:RemoveListener(eventName,action)
end

--这个是在有数据变化的地方调用的，不是测试中那样用的
function Event:Broadcast(eventName,...)
    MyEvent:Broadcast(eventName,...)
end