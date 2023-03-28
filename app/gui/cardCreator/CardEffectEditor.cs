using Godot;
using System;

public partial class CardEffectEditor : HBoxContainer
{
	public TemplateCardEffect TemplateCardEffect { get; set; }

	public TextureButton GoUp { get; private set; }
	public TextureButton GoDown { get; private set; }
	public CheckBox Visibility { get; private set; }
	public Label EffectName { get; private set; }
	public TextureButton Delete { get; private set; }

	public override void _Ready()
	{
		GoUp = GetNode<TextureButton>("GoUp");
		GoDown = GetNode<TextureButton>("GoDown");
		Visibility = GetNode<CheckBox>("Visibility");
		EffectName = GetNode<Label>("EffectName");
		Delete = GetNode<TextureButton>("Delete");
	}

	private void OnGoUpPressed()
    {

	}

	private void OnGoDownPressed()
	{

	}

	private void OnDeletePressed()
    {

    }

	private void OnVisibilityToggled(bool visible)
    {

    }
}
