using Godot;
using System;

public partial class NormalContainer : TemplateCardEffect
{
    public Label DefaultText { get; private set; }

    public override void _Ready()
    {
        DefaultText = GetNode<Label>("DefaultText");
    }

    public override void UpdateDamage(int value)
    {

    }

    public override void UpdateDon(int value)
    {

    }

    public override void UpdateText(string text)
    {
        DefaultText.Text = text;
    }
}
