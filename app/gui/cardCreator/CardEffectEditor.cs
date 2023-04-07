using Godot;
using System;
using System.Linq;

public partial class CardEffectEditor : VBoxContainer
{
	public TemplateCardEffect TemplateCardEffect { get; set; }

	public Container Default { get; private set; }
	public Container Extra { get; private set; }
	public TextureButton GoUp { get; private set; }
	public TextureButton GoDown { get; private set; }
	public CheckBox Visibility { get; private set; }
	public Label EffectName { get; private set; }
	public TextureButton Delete { get; private set; }
	public TextEdit TextEffect { get; private set; }
	public SpinBox DonEditor { get; private set; }

	[Signal]
	public delegate void CardEffectDeletedEventHandler(TemplateCardEffect templateCardEffect);

	[Signal]
	public delegate void ClickGoUpEventHandler(CardEffectEditor cardEffectEditor);

	[Signal]
	public delegate void ClickGoDownEventHandler(CardEffectEditor cardEffectEditor);

	public override void _Ready()
	{
		Default = GetNode<Container>("Default");
		Extra = GetNode<Container>("MarginContainer/Extra");

		GoUp = Default.GetNode<TextureButton>("GoUp");
		GoDown = Default.GetNode<TextureButton>("GoDown");
		Visibility = Default.GetNode<CheckBox>("Visibility");
		EffectName = Default.GetNode<Label>("EffectName");
		Delete = Default.GetNode<TextureButton>("Delete");

		TextEffect = Extra.GetNode<TextEdit>("TextEdit");
		DonEditor = Extra.GetNode<SpinBox>("DonEditor");
	}

	public void Update(CardCreatorEffectResource effectRes)
	{
		EffectName.Text = effectRes.EffectName;
		Extra.Visible = effectRes.HasDamage || effectRes.HasDon || effectRes.HasText;
		TextEffect.Visible = effectRes.HasText;
		DonEditor.Visible = effectRes.HasDon;
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

	private void OnTextEditChanged()
    {
		TemplateCardEffect?.UpdateText(TextEffect.Text);
    }

	private void OnDonEditorValueChanged(float value)
	{
		TemplateCardEffect?.UpdateDon((int) value);
	}
}
