var net = require('net')
var server = net.createServer(function(socket){
    console.log("客户端已经进来了，w(ﾟДﾟ)w")
    server.getConnections(function(err,count){
        console.log("当前存在%d个客户端连接",count)
        console.log("连接的客户端信息是%j",socket.address())
    })
})
server.on('connection',function(socket){
    socket.on('data',function(data){
        var buff = new Buffer.from(data)
        var buf1 = buff.slice(0,4)
        console.log("消息长度------>"+buf1.readIntLE(0,buf1.length))
        var buf2 = buff.slice(4,8)
        var msgId = buf2.readIntLE(0,buf2.length)
        console.log("消息号---->"+msgId)
        if(msgId === 1){
            var head = 4
            var toClientMsgId = 2
            var hBuff = new Buffer.alloc(4)
            hBuff.writeIntLE(head,0,4)
            var msgIdBuff = new Buffer.alloc(4)
            msgIdBuff.writeIntLE(toClientMsgId,0,4)
            var toClient = Buffer.concat([hBuff,msgIdBuff])
            socket.write(toClient,function(err){
                if(err){
                    console.error(err)
                }
            })
            console.log(toClient)
        }
        else{
            var head = 4
            var toClientMsgId = 1002
            var hBuff = new Buffer.alloc(4)
            hBuff.writeIntLE(head,0,4)
            var msgIdBuff = new Buffer.alloc(4)
            msgIdBuff.writeIntLE(toClientMsgId,0,4)
            var toClient = Buffer.concat([hBuff,msgIdBuff])
            socket.write(toClient,function(err){
                if(err){
                    console.error(err)
                }
            })
        }
        console.log("接收的字节数%d",data.length)
    })
    socket.on('end',function(){
        console.log("客户端关闭了连接")
    })
})
server.on('error',function(err){
    console.error("发生错误----->"+err)
})
server.listen(8888,'localhost',function(){
    console.log("服务器开始监听")
    var address = server.address()
    console.log("服务器的地址信息%j",address)
})