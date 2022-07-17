-- Count to 5 using a while loop and a control variable
local count = 0

while(count < 5)
do
    count = count + 1
    ui.StatusText = "Loop " .. count
    ui.ConsoleLog("Loop " .. count)    
    ui.Sleep(1000)
end

ui.ConsoleLog("Complete!")
ui.StatusText = "Complete!"