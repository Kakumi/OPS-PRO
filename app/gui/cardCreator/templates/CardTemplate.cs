using Godot;
using System;

public abstract partial class CardTemplate : TextureRect
{
	public override void _Ready()
	{
		base._Ready();
	}

	public abstract void UpdateCardTitle(string cardTitle);
	public abstract void UpdateCardNumber(string number);
	public abstract void UpdateCardRarity(string rarity);
	public abstract void UpdateCardPower(double power);
	public abstract void UpdateCardCounter(double counter);
	public abstract void UpdateCardType(string type);
	public abstract void UpdateCardAttribute(Texture2D texture);
	public abstract void UpdateCardCost(Texture2D texture);

	public abstract void UpdateCardTitlePx(double px);
	public abstract void UpdateCardNumberPx(double px);
	public abstract void UpdateCardPowerPx(double px);
	public abstract void UpdateCardCounterPx(double px);
	public abstract void UpdateCardTypePx(double px);

	public abstract void UpdateColor(Color color);

	public abstract void UpdateSecondaryColor(Color color);
}
