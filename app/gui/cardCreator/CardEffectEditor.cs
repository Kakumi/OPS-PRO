using Godot;
using System;
using System.Linq;

public partial class CardEffectEditor : HBoxContainer
{
	public TemplateCardEffect TemplateCardEffect { get; set; }
    public TextureButton GoUp { get; private set; }
	public TextureButton GoDown { get; private set; }
	public CheckBox Visibility { get; private set; }
	public Label EffectName { get; private set; }
	public TextureButton Delete { get; private set; }

	[Signal]
	public delegate void CardEffectDeletedEventHandler(TemplateCardEffect templateCardEffect);

	[Signal]
	public delegate void ClickGoUpEventHandler(CardEffectEditor cardEffectEditor);

	[Signal]
	public delegate void ClickGoDownEventHandler(CardEffectEditor cardEffectEditor);

	public override void _Ready()
	{
		GoUp = GetNode<TextureButton>("GoUp");
		GoDown = GetNode<TextureButton>("GoDown");
		Visibility = GetNode<CheckBox>("Visibility");
		EffectName = GetNode<Label>("EffectName");
		Delete = GetNode<TextureButton>("Delete");
	}

	public void Refresh(int position, int count)
    {
		UpdateArrows(position, count);
    }

	public void UpdateArrows(int position, int count)
    {
		if (count == 1)
		{
			GoUp.Disabled = true;
			GoDown.Disabled = true;
		}
		else
		{
			GoUp.Disabled = position == 0;
			GoDown.Disabled = position == (count - 1);
		}
	}

	private void OnGoUpPressed()
    {
		EmitSignal(SignalName.ClickGoUp, this);
	}

	private void OnGoDownPressed()
	{
		EmitSignal(SignalName.ClickGoDown, this);
	}

	private void OnDeletePressed()
    {
		TemplateCardEffect?.QueueFree();

		QueueFree();

		EmitSignal(SignalName.CardEffectDeleted, TemplateCardEffect);
	}

	private void OnVisibilityToggled(bool visible)
    {
		TemplateCardEffect.Visible = visible;
	}
}
