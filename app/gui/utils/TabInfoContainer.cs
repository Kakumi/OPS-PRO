using Godot;
using System;

public partial class TabInfoContainer : TabContainer
{
	public CardInfoTab CardInfoTab { get; private set; }
	public TabInfo Chat { get; private set; }
	public TabInfo Logs { get; private set; }

	public override void _Ready()
	{
		CardInfoTab = GetNode<CardInfoTab>("CardInfoTab");
		Chat = GetNode<TabInfo>("Chat");
		Logs = GetNode<TabInfo>("Logs");

		for(int i = 0; i < GetTabCount(); i++)
		{
			if (GetTabControl(i) is TabInfo)
			{
				var tab = (TabInfo)GetTabControl(i);
				SetTabTitle(i, tab.GetTabName());
			}
		}
	}
}
