using Godot;
using System;

public class DraggableWindow : Container
{
    private Vector2 _dragPosition = Vector2.Zero;
    
    public override void _Ready()
    {
        
    }

    public void OnGuiInput(InputEvent inputEvent)
    {
        GD.Print("test");
        if (inputEvent is InputEventMouseButton)
        {
            if (inputEvent.IsPressed())
            {
                GD.Print("set drag");
                _dragPosition = GetGlobalMousePosition() - RectGlobalPosition;
            } else
            {
                _dragPosition = Vector2.Zero;
            }
        } else if (inputEvent is InputEventMouseMotion && _dragPosition != Vector2.Zero)
        {
            GD.Print("remove drag");
            RectGlobalPosition = GetGlobalMousePosition() - _dragPosition;
        }
    }
}
