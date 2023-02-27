using Godot;
using System;

public class OPSPopup : Popup
{
    private string _title;
    private string _message;

    [Export]
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            UpdateTitle(value);
        }
    }

    [Export]
    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            UpdateMessage(value);
        }
    }

    public Label TitleLabel { get; protected set; }
    public Label MessageLabel { get; protected set; }

    public override void _Ready()
    {
        TitleLabel = GetNode<Label>("CenterContainer/AspectRatioContainer/PanelContainer/MarginContainer/VBoxContainer/Title");
        MessageLabel = GetNode<Label>("CenterContainer/AspectRatioContainer/PanelContainer/MarginContainer/VBoxContainer/Message");

        UpdateTitle(Title);
        UpdateMessage(Message);
    }

    private void UpdateTitle(string value)
    {
        if (TitleLabel != null)
        {
            TitleLabel.Visible = value != null;
            TitleLabel.Text = value;
        }
    }

    private void UpdateMessage(string value)
    {
        if (MessageLabel != null)
        {
            MessageLabel.Text = value;
        }
    }
}
