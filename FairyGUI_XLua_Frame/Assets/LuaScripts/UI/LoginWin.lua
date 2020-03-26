--登录界面
require 'UI/Core/WindowBase'
require 'Net/Tcp/TcpHelper'
require 'UI/Core/UIWindowMgr'
require 'Modules/LoginModule'
require 'Tools/Event'

LoginWin = {}
LoginWin = fgui.window_class(WindowBase)
LoginWin.Name = "LoginWin"

function LoginWin:OnInit()
	WindowBase.OnInit(LoginWin)
	LoginWin.window = self
	self.contentPane = LoginWin.contentPane

	LoginWin:Init()
	LoginWin:BindEvents()
end

function LoginWin:Init()
	self.LoginBtn = self:GetChild("LoginBtn")
	self.RegisterBtn = self:GetChild("RegisterBtn")
	self.UsernameInput = self:GetChild("UsernameInput")
	self.PasswordInput = self:GetChild("PasswordInput")
	
	--添加掉线监控测试
	function Callback(param)
		print("啊啊啊啊---->")
	end
	
	Event:AddEvent("OffLine",Callback)
	--测试网络延迟
	--require 'Tools/Timer'
	--require 'Net/Tcp/TcpHelper'
	--function NetDelay(param)
	--	print("UI打印的网络延迟----->"..TcpHelper.netDelay.."ms")
	--end
	--Timer:AddTimer(3,0,NetDelay,1)
end

function LoginWin:BindEvents()
	function OnLoginBtnClick()
		print(self.UsernameInput.text)
		print(self.PasswordInput.text)
		function OnOkBtn()
			print("OK")
			self.notice:Hide()
		end
		function OnCancel()
			print("Cancel")
			self.notice:Hide()
		end
		self.notice = UIWindowMgr:OpenNoticeWin("服务器爆炸了啊啊啊啊啊",OnOkBtn)
	end
	
	function OnRegisterBtnClick()
		TcpHelper:SendMsg(1001)
		
		self:Hide()
		require 'UI/RegisterWin'
		UIWindowMgr:OpenWindow("LoginModule",RegisterWin)
	end
	
	self.LoginBtn.onClick:Add(OnLoginBtnClick)
	self.RegisterBtn.onClick:Add(OnRegisterBtnClick)
end