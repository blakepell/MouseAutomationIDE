-- Save the original mouse coordinates so we can reset
-- them at the end
local originalX = mouse.X
local originalY = mouse.Y

local x = 0
local y = 0

for x = 1, 400 do
	y = y + 1
	mouse.SetPosition(x, y)
	ui.Pause(1)
end

for x = 400, 1, -1 do
	y = y + 1
	mouse.SetPosition(x, y)
	ui.Pause(1)
end

for x = 1, 400 do
	y = y - 1
	mouse.SetPosition(x, y)
	ui.Pause(1)
end

for x = 400, 1, -1 do
	y = y - 1
	mouse.SetPosition(x, y)
	ui.Pause(1)
end

-- Reset the coordinates where they were when the program started.
mouse.SetPosition(originalX, originalY)

ui.Log("Complete")