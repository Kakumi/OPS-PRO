using Godot;
using System;

public abstract partial class TemplateCardEffect : Container
{
    public abstract void UpdateText(string text);
    public abstract void UpdateDon(int value);
    public abstract void UpdateDamage(int value);
}
