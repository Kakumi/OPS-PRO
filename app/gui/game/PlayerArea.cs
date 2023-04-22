using Godot;
using System;

public partial class PlayerArea : VBoxContainer
{
	[Export]
	public NodePath GameboardPath { get; set; }

	public Playmat Playmat { get; private set; }
	public HandContainer Hand { get; private set; }
	public GamePlayerInfo PlayerInfo { get; private set; }
	public Gameboard Gameboard { get; private set; }

	public override void _Ready()
	{
		Playmat = GetNode<Playmat>("PlaymatContainer/Control/Playmat");
		Hand = GetNode<HandContainer>("HandContainer");
		PlayerInfo = GetNode<GamePlayerInfo>("PlaymatContainer/PlayerInfo");
		Gameboard = GetNode<Gameboard>(GameboardPath);

        Playmat.MouseEnterCard += Playmat_MouseEnterCard;
        Playmat.MouseExitCard += Playmat_MouseExitCard;
	}

    private void Playmat_MouseExitCard(Card card)
    {
		Gameboard.GameView.CardInfoPanel.ShowcardResource(card);
    }

    private void Playmat_MouseEnterCard(Card card)
    {
		Gameboard.GameView.CardInfoPanel.ShowcardResource(card);
    }
}
