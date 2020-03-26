require 'Config/Config'
require 'Debug'

UIWindowMgr = {}
UIWindowMgr.windows = {}
--打开指定窗口
--packageName UI资源所在的包
--windowClass  参数是指定的窗口类
function UIWindowMgr:OpenWindow(packageName,windowClass)
	local getWindow = self:GetWindow(windowClass)
	if getWindow then
		getWindow:Show()
		return getWindow
	else
		local window = {}
		-- window = windowClass.New(Config.uiPath..packageName)
		window = windowClass.New(Config.uiPath..packageName)
		window.name = windowClass.Name
		window:Show()
		table.insert(self.windows,window)
		return window
	end
end

function UIWindowMgr:CloseWindow(windowClass)
	local window = self:GetWindow(windowClass)
	if window ~= nil then
		window:Hide()
	end
end

function UIWindowMgr:Dispose(windowClass)
	local window = self:GetWindow(windowClass)
	if window ~= nil then
		window:Dispose()
	end
end

function UIWindowMgr:GetWindow(windowClass)
	for i=1,#UIWindowMgr.windows do
		local window = self.windows[i]
		if windowClass.Name == window.name then
			Debug.log("获取窗口"..window.name)
			return window
		end
	end
	return nil
end

function UIWindowMgr:OpenNoticeWin(msg,OnOk,OnCancel)
	require 'UI/NoticeWin'
	local window = self:OpenWindow("NoticeModule",NoticeWin)
	if window ~= nil then
		NoticeWin:SetMsg(msg,OnOk,OnCancel)
		window:BringToFront()
	end
	return window
end

--销毁当前场景中的所有窗体
--在加载场景完成之后调用，这个必须要调用
--不然内存爆炸
function UIWindowMgr:DisposeAllWindows()
	for i = 1, #self.windows do
		self.windows[i]:Dispose()
	end
	self.windows = {}
end

--没卵用
function UIWindowMgr:ToTop(windowClass)
	local window = self:GetWindow(windowClass)
	if window ~= nil then
		window:BringToFront()
	end
end