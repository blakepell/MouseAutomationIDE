local x = 0
local y = 0
local app = "teams"

ui.Log("Locating '" .. app .. "'")

local win = ui.GetWindowPosition(app)

-- Remember, goto statements in Lua have very specific rules, one of which
-- being local variables cannot be declared between the goto and where it
-- goes.  Declaring variables at the top scope helps in this case.
if win.X == -32000 or win.Y == -3200 then
	ui.Log("'" .. app .. "' was minimized.")
	goto continue
end

ui.Log("Found at " .. win.X .. ", " .. win.Y)

-- Save original mouse coordiates
x = mouse.X
y = mouse.Y

-- Set position (teams, then click), if the Y is negative meaning it's probably
-- on another monitor we have to set to 0, then move to the correct position.
if (win.Y < 0) then
	mouse.SetPosition(win.X + 16, 0)
end

mouse.SetPosition(win.X + 16, win.Y + 16)	
mouse.LeftClick()

-- Reset mouse position
mouse.SetPosition(x - 16, y - 16)
mouse.SetPosition(x, y)

::continue::

ui.Log("Complete!")