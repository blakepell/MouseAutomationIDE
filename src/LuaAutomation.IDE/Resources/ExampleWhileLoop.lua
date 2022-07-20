-- Count to 5 using a while loop and a control variable
local count = 0

while(count < 5)
do
    count = count + 1
    ui.StatusText = "Loop " .. count
    ui.Log("Loop " .. count)    

    -- Pause for 500 milliseconds
    ui.Sleep(500)
end

ui.Log("Complete!")
ui.StatusText = "Complete!"