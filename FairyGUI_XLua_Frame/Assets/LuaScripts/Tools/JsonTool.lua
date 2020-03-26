--Json工具类
--第一个功能，加载本地的Json文件，用于NetCode提示,还有配置表等
--第二个功能，解析服务器发过来的Json串儿
local json = require 'rapidjson'
require('ResMgr/ResMgr')
require 'Config/NetConfig'

JsonTool = {}

--数据配置表啥的，是公共资源
--其他需要本地化的数据是非公共资源
function JsonTool.LoadFile(fileName,bPublic)
    local content =  ResMgr:LoadBySyn(fileName,bPublic)
    local t = json.decode(content:ToString())
    return t
end 

function JsonTool.ParseString(str)
    return json.decode(str)
end