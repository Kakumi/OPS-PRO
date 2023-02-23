using Godot;
using System;

public class Tabs : TabContainer
{
    public CardInfoTab CardInfoTab { get; private set; }
    public Tab Chat { get; private set; }
    public Tab Logs { get; private set; }

    public override void _Ready()
    {
        CardInfoTab = GetNode<CardInfoTab>("CardInfoTab");
        Chat = GetNode<Tab>("Chat");
        Logs = GetNode<Tab>("Logs");

        for(int i = 0; i < GetTabCount(); i++)
        {
            if (GetTabControl(i) is Tab)
            {
                var tab = (Tab)GetTabControl(i);
                SetTabTitle(i, tab.GetTabName());
            }
        }
    }
}
