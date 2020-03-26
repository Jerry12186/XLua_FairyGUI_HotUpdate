require 'Data/DataBase'

UserData = DataBase:new()

--暂定一下这么多
UserData.username = "胡汉三" --玩家名称
UserData.userid = "1001"--玩家id，作为唯n一标识
UserData.diamond = 0 --钻石
UserData.levels = {} --玩家所有建筑的等级，包括人物的，vip等等级

--更新用户数据
function UserData:Update(playerInfo)
    --playerInfo 是从服务器那边传过来的
    self.username = playerInfo.username
    self.userid = playerInfo.userid
    self.diamond = playerInfo.diamond
    UserData.levels = playerInfo.levels
end