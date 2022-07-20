-- Count forward to 10
for i = 1, 10 do
    ui.Log(i)

    -- Pause for 250 milliseconds
    ui.Sleep(250)
end

-- Count backwards 10 to 1
for i = 10, 1, -1 do
    ui.Log(i)

    -- Pause for 250 milliseconds
    ui.Sleep(250)
end

-- Example of looping over an array with values
local values = {
    "Value 1",
    "Value 2",
    "Value 3",
    "Value 4",
    "Value 5"
}

-- i is the iteration of the loop you're on, if you don't need it
-- you can replace it with a discard _
for i, value in ipairs(values) do
	ui.Log("Element " .. i .. " = " .. value)
	ui.Sleep(250)
end

ui.Log("Complete!")