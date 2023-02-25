using Godot;
using System;

public class Tab : Godot.Tabs
{
    [Export(PropertyHint.Enum)]
    public TabType Type { get; set; }

    [Export(PropertyHint.Range, "1, 500, 1")]
    public int Capacity { get; set; } = 100;

    [Export]
    public DynamicFont Font { get; set; }

    [Export]
    public bool UseNotifier { get; set; } = false;

    [Export]
    public string NotifierCode { get; set; }

    protected ScrollContainer ScrollContainer { get; private set; }
    protected VBoxContainer Texts { get; private set; }

    public override void _Ready()
    {
        ScrollContainer = GetNode<ScrollContainer>("MarginContainer/ScrollContainer");
        Texts = ScrollContainer.GetNode<VBoxContainer>("Texts");

        if (UseNotifier)
        {
            NotifierManager.Instance.Listen(NotifierCode, NotifierReceiveMessage);
        }
    }

    public override void _ExitTree()
    {
        if (UseNotifier)
        {
            NotifierManager.Instance.StopListener(NotifierCode, NotifierReceiveMessage);
        }
    }

    protected virtual void NotifierReceiveMessage(object[] obj)
    {
        if (obj.Length > 0)
        {
            Add(obj[0].ToString());
        }
    }

    public virtual void OnTextsResized()
    {
        if (ScrollContainer != null)
        {
            ScrollContainer.GetVScrollbar().Value = ScrollContainer.GetVScrollbar().MaxValue;
        }
    }

    public void Add(string text)
    {
        if (Texts != null)
        {
            var label = new RichTextLabel();
            Texts.AddChild(label);
            label.BbcodeEnabled = true;
            label.FitContentHeight = true;
            label.Text = text;
            label.AddFontOverride("normal_font", Font);

            if (Texts.GetChildCount() > Capacity)
            {
                Texts.GetChild(0).QueueFree();
            }
        }
    }

    public string GetTabName()
    {
        switch (Type)
        {
            case TabType.CardInfo: return "Information";
            case TabType.Chat: return "Chat";
            case TabType.Log: return "Logs";
        }

        return "None";
    }
}
