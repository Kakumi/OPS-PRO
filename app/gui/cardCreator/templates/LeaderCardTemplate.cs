using Godot;
using System;

public partial class LeaderCardTemplate : CardTemplate
{
    public Label CardTitle { get; private set; }
    public Label Number { get; private set; }
    public Label Rarity { get; private set; }
    public Label Power { get; private set; }
    public Label Type { get; private set; }
    public TextureRect Attribute { get; private set; }
    public TextureRect Cost { get; private set; }
    public PanelContainer CardTextPanel { get; private set; }
    public Container EffectList { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        CardTitle = GetNode<Label>("CardTitle");
        Number = GetNode<Label>("Number");
        Rarity = GetNode<Label>("Rarity");
        Power = GetNode<Label>("Power");
        Type = GetNode<Label>("Type");
        Attribute = GetNode<TextureRect>("Attribute");
        Cost = GetNode<TextureRect>("Cost");
        CardTextPanel = GetNode<PanelContainer>("CardTextPanel");
        EffectList = CardTextPanel.GetNode<Container>("EffectList");
    }

    public override void UpdateCardTitle(string cardTitle)
    {
        CardTitle.Text = cardTitle;
    }

    public override void UpdateCardNumber(string number)
    {
        Number.Text = number;
    }

    public override void UpdateCardRarity(string rarity)
    {
        Rarity.Text = rarity;
    }

    public override void UpdateCardPower(double power)
    {
        Power.Text = power.ToString();
    }

    public override void UpdateCardCounter(double counter)
    {
    }

    public override void UpdateCardType(string type)
    {
        Type.Text = type;
    }

    public override void UpdateCardAttribute(Texture2D texture)
    {
        Attribute.Texture = texture;
    }

    public override void UpdateCardCost(Texture2D texture)
    {
        Cost.Texture = texture;
    }

    public override void UpdateCardTitlePx(double px)
    {
        CardTitle.Set("theme_override_font_sizes/font_size", px);
    }

    public override void UpdateCardNumberPx(double px)
    {
        Number.Set("theme_override_font_sizes/font_size", px);
    }

    public override void UpdateCardPowerPx(double px)
    {
        Power.Set("theme_override_font_sizes/font_size", px);
    }

    public override void UpdateCardCounterPx(double px)
    {
    }

    public override void UpdateCardTypePx(double px)
    {
        Type.Set("theme_override_font_sizes/font_size", px);
    }

    public override void UpdateColor(Color color)
    {
        CardTitle.Modulate = color;
        Number.Modulate = color;
        Power.Modulate = color;
        Cost.Modulate = color;
    }

    public override void UpdateSecondaryColor(Color color)
    {
        Rarity.Set("theme_override_colors/font_color", color);
        Rarity.Set("theme_override_colors/font_outline_color", color);
    }

    public override void AddEffect(TemplateCardEffect effect)
    {
        EffectList.AddChild(effect);
    }

    public override void UpdateEffectBackgroundVisibility(bool visible)
    {
        var stylebox = CardTextPanel.GetThemeStylebox("panel") as StyleBoxFlat;
        if (stylebox != null)
        {
            if (visible)
            {
                stylebox.BgColor = new Color(stylebox.BgColor.R, stylebox.BgColor.G, stylebox.BgColor.B, 0.5f);
            }
            else
            {
                stylebox.BgColor = new Color(stylebox.BgColor.R, stylebox.BgColor.G, stylebox.BgColor.B, 0f);
            }
        }
    }
}
