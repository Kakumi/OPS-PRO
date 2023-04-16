using Godot;
using System;

public partial class Gameboard : VBoxContainer
{
	public Playmat OpponentPlaymat { get; private set; }
	public Playmat MyPlaymat { get; private set; }

	[Signal]
	public delegate void MouseEnterCardEventHandler(Card card);

	[Signal]
	public delegate void MouseExitCardEventHandler(Card card);

	public override void _Ready()
	{
		OpponentPlaymat = GetNode<Playmat>("OpponentSide/OpponentPlaymat");
		MyPlaymat = GetNode<Playmat>("MySide/MyPlaymat");

        OpponentPlaymat.MouseEnterCard += OpponentPlaymat_MouseEnterCard;
        OpponentPlaymat.MouseExitCard += OpponentPlaymat_MouseExitCard;
        MyPlaymat.MouseEnterCard += MyPlaymat_MouseEnterCard;
        MyPlaymat.MouseExitCard += MyPlaymat_MouseExitCard;
    }

    private void OpponentPlaymat_MouseEnterCard(Card card)
    {
        EmitSignal(SignalName.MouseEnterCard, card);
    }

    private void OpponentPlaymat_MouseExitCard(Card card)
    {
        EmitSignal(SignalName.MouseExitCard, card);
    }

    private void MyPlaymat_MouseEnterCard(Card card)
    {
        EmitSignal(SignalName.MouseEnterCard, card);
    }

    private void MyPlaymat_MouseExitCard(Card card)
    {
        EmitSignal(SignalName.MouseExitCard, card);
    }

    private void Test1()
    {
        MyPlaymat.LeaderSlotCard.Card.ToggleFlip();
    }

    private void Test2()
    {
        MyPlaymat.LeaderSlotCard.Card.ToggleRest();
    }
}
