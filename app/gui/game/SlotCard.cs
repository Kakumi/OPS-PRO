using Godot;
using Serilog;
using System;

public partial class SlotCard : TextureRect
{
	public Guid Guid { get; private set; }
	public Card Card { get; private set; }
	public bool Selected { get; private set; }
	public MenuButton Options { get; private set; }

	public override void _Ready()
	{
		Guid = Guid.NewGuid();
		Card = GetNode<Card>("MarginContainer/Card");
		Options = GetNode<MenuButton>("Options");
		Options.GetPopup().IdPressed += OnOptionsPressed;

		Selected = false;
		//Card.CardResourceUpdated += Card_CardResourceUpdated;
		//Card_CardResourceUpdated(Card.CardResource);
	}

    private void Card_CardResourceUpdated(CardResource cardResource)
    {
		//Options.Disabled = cardResource == null;
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

	private void OnOptionsPressed(long id)
	{

	}
}
