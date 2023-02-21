using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class CardInfoPanel : VBoxContainer
{
    protected TextureRect CardImg { get; set; }
    protected Tabs Tabs { get; private set; }

    public override void _Ready()
    {
        CardImg = GetNode<TextureRect>("TopContainer/CardImg");
        Tabs = GetNode<Tabs>("Tabs");
    }

    public void ShowCardInfo(CardInfo cardInfo)
    {
        Task.Run(async () =>
        {
            CardImg.Texture = await CardManager.Instance.DownloadAndGetTexture(cardInfo);
        });

        Tabs.CardInfoTab.ShowCardInfo(cardInfo);
    }
}
