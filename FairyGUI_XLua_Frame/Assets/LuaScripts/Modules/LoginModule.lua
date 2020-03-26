local pb = require 'pb'
require('Net/Tcp/TcpHelper')
require 'Config/NetConfig'

LoginModule = {}

LoginModule.serverIp = "127.0.0.1"
LoginModule.port = 8888

--这是进入初始化登录场景进行调用
function LoginModule:InitNet()
    --function Callback(isOk)
    --    if isOk then
    --        print("连接服务器成功(*^▽^*)")
    --        --这个是测试长连接的，测试成功
    --        TcpHelper:SendMsg(1001) 
    --    else
    --        print("服务器连接失败o(╥﹏╥)o")
    --    end
    --end
    --连接到服务器，如果本地有Session，则发送到服务器，方便获取玩家信息
    --同步连接服务器
    --TcpHelper:ConnectBySync(LoginModule.serverIp,LoginModule.port)
    
    --异步连接服务器
    TcpHelper:ConnectByAsync(LoginModule.serverIp,LoginModule.port)
end