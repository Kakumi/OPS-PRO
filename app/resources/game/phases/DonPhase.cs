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

    public Task OnPhaseEnded(PlayerArea playerArea)
    {
        return Task.CompletedTask;
    }

    public Task OnPhaseStarted(PlayerArea playerArea)
    {
        if (playerArea.FirstToPlay && playerArea.Gameboard.TurnCounter == 1)
        {
            playerArea.Playmat.DrawDonCard(1);
        } else
        {
            playerArea.Playmat.DrawDonCard(2);
        }

        return Task.CompletedTask;
    }

    public bool IsAutoNextPhase()
    {
        return true;
    }
}
