using Godot;
using System;

public partial class DisplayGrabMode : Label
{
    public void OnGrabModeChanged(bool isToggleGrabMode)
    {
        Text = isToggleGrabMode ? "Grab Mode: Toggle" : "Grab Mode: Hold";

    }
}
