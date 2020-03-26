--计时器
--很好用的计时器
require "FairyGUI"
Timer = {}
--添加定时器
--interval 几秒后调用
--repeatCount 调用几次，0是无数次
--func 回调方法
--param 参数
function Timer:AddTimer(interval,repeatCount,func,param)
	fgui.add_timer(interval,repeatCount,func,nil,param)
end

function Timer:RemoveTimer(callback)
	fgui.remove_timer(callback,nil)
end