-- An example program that shows some of the ways that a script can update
-- the window.
ui.Log("Starting")
ui.Sleep(1000)

ui.Log("Setting status text")
ui.Sleep(1000)
ui.StatusText = "This is a new status text message"

ui.Log("Maximize window")
ui.Sleep(1000)
ui.Maximize()

ui.Log("Minimize window")
ui.Sleep(1000)
ui.Minimize()

ui.Log("Restoring window (activate)")
ui.Sleep(1000)
ui.Activate()

ui.Log("Set Window Title")
ui.Sleep(1000)
ui.Title = "Untitled"

ui.Log("Complete")