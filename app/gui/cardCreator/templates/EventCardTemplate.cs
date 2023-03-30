using Godot;
using System;

public partial class EventCardTemplate : CardTemplate
{
    public Label CardTitle { get; private set; }
    public Label Number { get; private set; }
    public Label Rarity { get; private set; }
    public Label Type { get; private set; }
    public TextureRect Cost { get; private set; }
    public Container EffectList { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        CardTitle = GetNode<Label>("CardTitle");
        Number = GetNode<Label>("Number");
        Rarity = GetNode<Label>("Rarity");
        Type = GetNode<Label>("Type");
        Cost = GetNode<TextureRect>("Cost");
        EffectList = GetNode<Container>("EffectList");
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
        Cost.Modulate = color;
    }

    public override void UpdateSecondaryColor(Color color)
    {
        Rarity.Set("theme_override_colors/font_color", color);
        Rarity.Set("theme_override_colors/font_outline_color", color);
    }

    public override bool AddEffect(TemplateCardEffect effect)
    {
        EffectList.AddChild(effect);
        return true;
    }

    public override void UpdateEffectBackgroundVisibility(bool visible)
    {
    }

    public override bool MoveUp(TemplateCardEffect effect)
    {
        var indexOf = EffectList.GetChildren().IndexOf(effect);
        if (indexOf > 0)
        {
            EffectList.MoveChild(effect, indexOf - 1);
            return true;
        }

        return false;
    }

    public override bool MoveDown(TemplateCardEffect effect)
    {
        var indexOf = EffectList.GetChildren().IndexOf(effect);
        if (indexOf < (EffectList.GetChildCount() - 1))
        {
            EffectList.MoveChild(effect, indexOf + 1);
            return true;
        }

        return false;
    }
}
