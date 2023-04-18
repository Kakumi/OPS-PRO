using Godot;
using Serilog;
using System;

public partial class SlotCard : TextureRect
{
	public Guid Guid { get; private set; }
	public Card Card { get; private set; }
	public bool Selected { get; private set; }
	public OptionButton Options { get; private set; }

	public override void _Ready()
	{
		Guid = Guid.NewGuid();
		Card = GetNode<Card>("MarginContainer/Card");
		Options = GetNode<OptionButton>("Options");

		Selected = false;
		Options.Hide();
	}

	public void ToggleSelection()
    {
		Selected = !Selected;
		if (Selected)
        {
			Select();
        } else
        {
			Unselect();
        }
    }

	public void Select()
    {
		SelfModulate = new Color(255, 0, 0, 1);
	}

	public void Unselect()
	{
		SelfModulate = new Color(255, 255, 255, 1);
	}

	private void OnLeftClickCard(CardResource cardResource)
    {
		Options.Show();
	}

	private void OnOptionsToggled(bool pressed)
    {
		if (!pressed)
		{
			Options.Hide();
		}
    }

	private void OnMouseExited()
	{
		Options.Hide();
	}
}
