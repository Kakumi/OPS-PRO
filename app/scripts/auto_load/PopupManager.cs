using Godot;
using System;

public class PopupManager : Node
{
    [Export]
    public PackedScene PopupStyle { get; set; }

    public CanvasLayer CanvasLayer { get; protected set; }

    private static PopupManager _instance;
    public static PopupManager Instance => _instance;
    private Config _config;

    public override void _Ready()
    {
        _instance = this;
        CanvasLayer = GetNode<CanvasLayer>("CanvasLayer");

        _config = new Config();
        SettingsManager.Instance.ReadConfig(_config);
    }

    public OPSPopup CreatePopup(string title, string message)
    {
        var popup = PopupStyle.Instance<OPSPopup>();
        popup.Title = title;
        popup.Message = message;
        popup.Theme = GD.Load<Theme>(_config.Theme);

        CanvasLayer.AddChild(popup);


        return popup;
    }
}
