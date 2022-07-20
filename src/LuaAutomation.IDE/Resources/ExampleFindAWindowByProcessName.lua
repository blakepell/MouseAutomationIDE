-- The process name which is the executable minus .EXE
local win = ui.GetWindowPosition("LuaAutomation.IDE")

-- 0, 0 could mean 0, 0 or it could mean the window wasn't found.
-- -32000, -32000 means the window as found but is minimized

ui.Log("Lua Automation IDE: x:" .. win.X .. " y:" .. win.Y)

ui.Sleep(1000)
ui.Minimize()

win = ui.GetWindowPosition("LuaAutomation.IDE")

if win.X == -32000 or win.Y == -32000 then
	ui.Log("Window was minimized at the time of this call.")
end

ui.Sleep(1000)
ui.Activate()

win = ui.GetWindowPosition("LuaAutomation.IDE")

if win.X >= 0 or win.Y >= 0 then
	ui.Log("Window was restored at: x:" .. win.X .. " y:" .. win.Y)
end
