using Godot;
using Godot.Collections;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial class CardInfoPanel : VBoxContainer
{
	[Export]
	public Array<NodePath> HoverCardNodePath { get; set; }

	protected TextureRect CardImg { get; set; }
	protected TabInfoContainer TabBar { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        CardImg = GetNode<TextureRect>("TopContainer/CardImg");
        TabBar = GetNode<TabInfoContainer>("TabBar");

        HoverCardNodePath.ToList().ForEach(path =>
        {
            var node = GetNode(path);
            if (node.HasSignal("MouseEnterCard"))
            {
                node.Connect("MouseEnterCard", new Callable(this, nameof(CardMouseEntered)));
                //node.ConnectIfMissing("MouseEnterCard", this, nameof(CardMouseEntered));
            }

            if (node.HasSignal("MouseExitCard"))
            {
                node.Connect("MouseExitCard", new Callable(this, nameof(CardMouseExited)));
                //node.ConnectIfMissing("MouseExitCard", this, nameof(CardMouseExited));
            }
        });
    }

    public void CardMouseEntered(Card card)
	{
		ShowcardResource(card);
	}

	public void CardMouseExited(Card card)
	{
		
	}

	public void ShowcardResource(Card card)
	{
		CardImg.Texture = card.Texture;

		TabBar.CardInfoTab.ShowcardResource(card.CardResource);
	}
}
