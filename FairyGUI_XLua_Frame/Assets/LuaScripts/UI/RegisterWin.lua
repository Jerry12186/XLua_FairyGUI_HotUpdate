--注册界面
require 'UI/Core/WindowBase'
require 'UI/Core/UIWindowMgr'

RegisterWin = {}
RegisterWin = fgui.window_class(WindowBase)
RegisterWin.Name = "RegisterWin"

function RegisterWin:OnInit()
    WindowBase.OnInit(RegisterWin)
    RegisterWin.window = self
    self.contentPane = RegisterWin.contentPane
    
    RegisterWin:Init()
    RegisterWin:BindEvents()
end

function RegisterWin:Init()
    self.RegisterBtn = self:GetChild("RegisterBtn")
    self.UsernameInput = self:GetChild("UsernameInput")
    self.PasswordInput = self:GetChild("PasswordInput")
    self.OkPasswordInput = self:GetChild("OkPasswordInput")
end

function RegisterWin:BindEvents()
    function OnRegisterBtnClick()
        --测试post请求
        --测试成功
        require ('Net/Http/HttpHelper')
        HttpHelper:AddField("name","Jerry")
        HttpHelper:AddField("age",22)
        function Callback(text)
            print(text)
        end
        HttpHelper:Post("http://127.0.0.1:8888",Callback)

        local username = self.UsernameInput.text
        if self.PasswordInput.text == self.OkPasswordInput.text then
            local password = self.PasswordInput.text
           print("--这里向服务器发送消息") 
        else
            print("输入的密码不一致")
        end
    end
    
    self.RegisterBtn.onClick:Add(OnRegisterBtnClick)
end