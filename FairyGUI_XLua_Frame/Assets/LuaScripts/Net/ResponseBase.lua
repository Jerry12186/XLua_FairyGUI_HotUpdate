--回应消息的基类
--之后的登录消息，还是其他消息都要继承该方法
ResponseBase = {}

function ResponseBase:new(o)
	o = o or {}
	setmetatable(o, { __index = self })   
    return o
end

function ResponseBase:HandleMsg(msg)
	-- body
end