using Godot;
using System;

public partial class GamePlayerInfo : PanelContainer
{
    [Export]
    public NodePath PlayerAreaPath { get; set; }

    public PlayerArea PlayerArea { get; private set; }

    public RichTextLabel InfoMessage { get; private set; }

	public override void _Ready()
    {
        PlayerArea = GetNode<PlayerArea>(PlayerAreaPath);

        InfoMessage = GetNode<RichTextLabel>("MarginContainer/VBoxContainer/Bottom/RichTextLabel");
	}

    public void UpdateMessage(string message, string color)
    {
        if (message == null)
        {
            InfoMessage.Text = null;
        } else
        {
            InfoMessage.Text = $"[center][color={color}]{Tr(message)}[/color][/center]";
        }
    }
}
