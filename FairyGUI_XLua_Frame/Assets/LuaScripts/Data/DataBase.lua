--数据基类，用户动态更新的数据
DataBase = {}

function DataBase:new(o)
    o = o or {}
    setmetatable(o,{__index = self})
    return o
end

function DataBase:Init()
    
end

function DataBase:Dispose()
    
end