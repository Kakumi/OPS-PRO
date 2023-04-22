using Godot;
using System;

public partial class GameView : HBoxContainer
{
	public Gameboard Gameboard { get; private set; }
	public CardInfoPanel CardInfoPanel { get; private set; }

	public override void _Ready()
	{
		Gameboard = GetNode<Gameboard>("VBoxContainer/Gameboard");
		CardInfoPanel = GetNode<CardInfoPanel>("CardInfoPanel");
	}
}
