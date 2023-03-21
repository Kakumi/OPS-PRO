using Godot;
using System;

public partial class DraggableWindow : Container
{
    private Vector2 _dragPosition = Vector2.Zero;
    
    public override void _Ready()
    {
        
    }

    public void OnGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton)
        {
            if (inputEvent.IsPressed())
            {
                _dragPosition = GetGlobalMousePosition() - GlobalPosition;
            } else
            {
                _dragPosition = Vector2.Zero;
            }
        } else if (inputEvent is InputEventMouseMotion && _dragPosition != Vector2.Zero)
        {
            GlobalPosition = GetGlobalMousePosition() - _dragPosition;
        }
    }
}
