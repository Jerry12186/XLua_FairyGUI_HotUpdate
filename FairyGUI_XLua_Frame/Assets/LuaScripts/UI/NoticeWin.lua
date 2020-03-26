require 'UI/Core/WindowBase'

NoticeWin = {}
NoticeWin = fgui.window_class(WindowBase)
NoticeWin.Name = "NoticeWin"

function NoticeWin:OnInit()
    WindowBase.OnInit(NoticeWin)
    NoticeWin.window = self
    self.contentPane = NoticeWin.contentPane
    
    NoticeWin:Init()
    NoticeWin:BindEvents()
end

function NoticeWin:Init()
    self.ctrl = self:GetController("c1")
    self.msg = self:GetChild("Msg")
    self.OKBtn = self:GetChild("OKBtn")
    self.CancelBtn = self:GetChild("CancelBtn")
end

function NoticeWin:BindEvents()
    
end

function NoticeWin:SetMsg(msg,OnOkBtn,OnCancelBtn)
    self.msg.text = msg
    if OnCancelBtn == nil then
        self.ctrl.selectedIndex = 1
        self.OKBtn.onClick:Add(OnOkBtn)
    else
        self.ctrl.selectedIndex = 0
        self.OKBtn.onClick:Add(OnOkBtn)
        self.CancelBtn.onClick:Add(OnCancelBtn)
    end
    
end