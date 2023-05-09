using Godot;
using Serilog;
using System;
using System.Threading.Tasks;

public partial class PlayerArea : VBoxContainer
{
	[Export]
	public NodePath GameboardPath { get; set; }

	public Guid UserId { get; set; }

	public Playmat Playmat { get; private set; }
	public HandContainer Hand { get; private set; }
	public GamePlayerInfo PlayerInfo { get; private set; }
	public Gameboard Gameboard { get; private set; }
	public IPhase CurrentPhase { get; private set; }
	public bool FirstToPlay { get; internal set; }

	public override void _Ready()
	{
		Playmat = GetNode<Playmat>("PlaymatContainer/Control/Playmat");
		Hand = GetNode<HandContainer>("HandContainer");
		PlayerInfo = GetNode<GamePlayerInfo>("PlaymatContainer/PlayerInfo");
		Gameboard = GetNode<Gameboard>(GameboardPath);

        Playmat.MouseEnterCard += Playmat_MouseEnterCard;
        Playmat.MouseExitCard += Playmat_MouseExitCard;

		FirstToPlay = true;
	}

    private void Playmat_MouseExitCard(Card card)
    {
		Gameboard.GameView.CardInfoPanel.ShowcardResource(card);
    }

    private void Playmat_MouseEnterCard(Card card)
    {
		Gameboard.GameView.CardInfoPanel.ShowcardResource(card);
	}

	public async Task UpdatePhase(IPhase phase)
	{
		Log.Debug("Update phase from {CurrentPhase} to {Phase}", CurrentPhase, phase);

		if (CurrentPhase != null)
		{
			await CurrentPhase.OnPhaseEnded(this);
		}

		CurrentPhase = phase;
		UpdateSlotCardsAction(phase);

		Gameboard.NextPhaseButton.Text = Tr(phase.GetTrKeyNextPhase());
		Gameboard.NextPhaseButton.Disabled = phase.NextPhase() == null;

		await phase.OnPhaseStarted(this);

		if (phase.IsAutoNextPhase())
        {
			await UpdatePhase(phase.NextPhase());
        }
	}

	public void UpdateSlotCardsAction(IPhase phase)
    {
		Log.Debug($"Updating Slot Cards actions");

		Hand.GetCards().ForEach(x => x.CardActionUpdated(phase));
		Playmat.LeaderSlotCard.CardActionUpdated(phase);
		Playmat.StageSlotCard.CardActionUpdated(phase);
		Playmat.DeckSlotCard.CardActionUpdated(phase);
		Playmat.TrashSlotCard.CardActionUpdated(phase);
		Playmat.LifeSlotCard.CardActionUpdated(phase);
		Playmat.DonDeckSlotCard.CardActionUpdated(phase);
		Playmat.CostSlotCard.CardActionUpdated(phase);
		Playmat.CharactersSlots.ForEach(x => x.CardActionUpdated(phase));
	}
}
