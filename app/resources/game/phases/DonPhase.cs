using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DonPhase : IPhase
{
    public string GetTrKeyNextPhase()
    {
        return "PHASE_DON_GO_NEXT";
    }

    public bool IsActionAllowed(CardSelectorSource source, CardSelectorAction action)
    {
        return action == CardSelectorAction.See;
    }

    public IPhase NextPhase()
    {
        return new MainPhase();
    }

    public void OnPhaseEnded(PlayerArea playerArea)
    {
    }

    public void OnPhaseStarted(PlayerArea playerArea)
    {
        if (playerArea.FirstToPlay && playerArea.Gameboard.TurnCounter == 1)
        {
            //Draw 1 DON
        } else
        {
            //Draw 2 DON
        }

        playerArea.UpdatePhase(NextPhase());
    }
}
