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

ui.Log("Complete!")