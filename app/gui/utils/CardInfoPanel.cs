using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class CardInfoPanel : VBoxContainer
{
    [Export]
    public List<NodePath> HoverCardNodePath { get; set; }

    protected TextureRect CardImg { get; set; }
    protected Tabs Tabs { get; private set; }

    public override void _Ready()
    {
        CardImg = GetNode<TextureRect>("TopContainer/CardImg");
        Tabs = GetNode<Tabs>("Tabs");

        HoverCardNodePath.ForEach(path =>
        {
            var node = GetNode(path);
            if (node.HasSignal("MouseEnterCard"))
            {
                node.ConnectIfMissing("MouseEnterCard", this, nameof(CardMouseEntered));
            }

            if (node.HasSignal("MouseExitCard"))
            {
                node.ConnectIfMissing("MouseExitCard", this, nameof(CardMouseExited));
            }
        });
    }

    public void CardMouseEntered(Card card)
    {
        ShowCardInfo(card);
    }

    public void CardMouseExited(Card card)
    {
        
    }

    public void ShowCardInfo(Card card)
    {
        CardImg.Texture = card.Texture;

        Tabs.CardInfoTab.ShowCardInfo(card.CardInfo);
    }
}
