ui.Log("Starting")

-- Opens a file in write mode which will truncate it's contents.
-- Note you need to change the path to somewhere with a folder that
-- exists.
file = io.open("C:\\Temp\\test.lua", "w")
io.output(file)
io.write("-- This is a test comment...")
io.close()

-- Opens a file in read
file = io.open("C:\\Temp\\test.lua", "r")
io.input(file)
local buf = file.read()
file.close(file)

if (buf == null) then
	ui.ConsoleLog("buf was null.")
end

ui.Log(buf)
ui.Log("Complete")