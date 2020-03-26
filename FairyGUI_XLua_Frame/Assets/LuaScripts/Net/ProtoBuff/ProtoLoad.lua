--加载ProtocolBuff文件
require('Config/NetConfig')
require('ResMgr/ResMgr')
local protoc = require 'protoc'

ProtoLoad = {}
ProtoLoad.proTable = {}--加入一个table表，防止重复加载
function ProtoLoad:loadfile(fileName)
    if ProtoLoad.proTable[fileName] == nil then
        local content = ResMgr:LoadBySyn(NetConfig.ProtocolBuffPath..fileName,true)
        assert(protoc:load(content:ToString()))
        ProtoLoad.proTable[fileName] = fileName
    end
end