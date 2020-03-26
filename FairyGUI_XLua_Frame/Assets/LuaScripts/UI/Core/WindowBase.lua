require "FairyGUI"

--UI窗口基类
--窗口类，名副其实，在FairyGUI中都是以窗口的形式显示的
WindowBase = {}
WindowBase = fgui.window_class()
WindowBase.window = nil
WindowBase.ItemRenderer = nil

local ResHelper = CS.MyABSys.ResHelper
--构建函数
function WindowBase:ctor(packageName)
	local index = string.find(packageName,"/")
	WindowBase.packageName = string.sub(packageName,index + 1)
	-- UIPackage.AddPackage(packageName)
	-- Debug.log(packageName)
	ResHelper.Instance:LoadUI(packageName)
end
--此方法中的
--子类中的self是LuaWindow类
--子类的此方法不能用self
--我也不知道为啥
--别问，问就乔碧萝

--如果当前窗口有两个或者多个不同list，list的itemProvider、itemRenderer和OnListItemClick
--请在子类中设置其他的
function WindowBase:OnInit()
	--现在的self是子类，不是LuaWindow类
	WindowBase.chileClass = self
	self.contentPane = UIPackage.CreateObject(self.packageName, self.Name)
	self.contentPane.fairyBatching = true
	self.contentPane:MakeFullScreen()
	--这个是可选项
	--一般用来做聊天用的
	-- function ListItemRes(index)
	-- 	return ""
	-- end
	self.ListItemRes = nil
	--如果使用列表，必须给这个方法赋值
	-- function ItemRenderer(index,obj)
	-- 	Debug.log("父类开始渲染list")
	-- end
	self.ItemRenderer = nil
	self.OnListItemClick = nil

    Debug.log("窗口父类初始化")
end

--获取子物体
function WindowBase:GetChild(childName)
	return self.contentPane:GetChild(childName)
end

--获取控制器
function WindowBase:GetController(ctrlName)
	return self.contentPane:GetController(ctrlName)
end

--如果UI有List，使用此方法初始化
--list 中所有点击事件不要用 function...end
--初始化之前，需要把itemRenderer、itemProvider(如果有)、OnListItemClick(如果有)先赋值
--用的时候再初始化，不要提前初始化
function WindowBase:InitList(listName)
	local list = self:GetChild(listName)
	list:SetVirtual()--设置成虚拟列表，节省资源
	if self.ListItemRes ~= nil then
		list.itemProvider = self.ListItemRes
	end
	if self.ItemRenderer ~= nil then
		list.itemRenderer = self.ItemRenderer
	end
	if self.OnListItemClick ~= nil then
		list.onClickItem:Add(self.OnListItemClick)
	end
	return list
end



function WindowBase:OnShown()
	Debug.log("窗口父类----->打开")
end

function WindowBase:OnHide()
	Debug.log("窗口父类----->关闭")
end
--关闭窗口，只是隐藏，并没销毁
function WindowBase:Hide()
	self.window:Hide()
end
--销毁窗口
function WindowBase:Dispose(self)
	self.window:Dispose()
end