using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DrawPhase : IPhase
{
    public string GetTrKeyNextPhase()
    {
        return "PHASE_DRAW_GO_NEXT";
    }

    public bool IsActionAllowed(CardSelectorSource source, CardSelectorAction action)
    {
        return action == CardSelectorAction.See;
    }

    public IPhase NextPhase()
    {
        return new DonPhase();
    }

    public void OnPhaseEnded(PlayerArea playerArea)
    {
    }

    public void OnPhaseStarted(PlayerArea playerArea)
    {
        if (playerArea.FirstToPlay)
        {
            playerArea.Gameboard.IncrementTurn();
        }

        if (!playerArea.FirstToPlay || playerArea.Gameboard.TurnCounter != 1)
        {
            playerArea.Playmat.DrawCard();
        }

        playerArea.UpdatePhase(NextPhase());
    }
}