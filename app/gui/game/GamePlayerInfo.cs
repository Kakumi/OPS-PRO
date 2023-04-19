using Godot;
using System;

public partial class GamePlayerInfo : PanelContainer
{
    public RichTextLabel InfoMessage { get; private set; }

	public override void _Ready()
	{
        InfoMessage = GetNode<RichTextLabel>("MarginContainer/VBoxContainer/Bottom/RichTextLabel");
	}

    public void UpdateMessage(string message, string color)
    {
        InfoMessage.Text = $"[center][color={color}]{Tr(message)}[/color][/center]";
    }
}
