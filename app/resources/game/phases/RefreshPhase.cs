using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RefreshPhase : IPhase
{
    public string GetTrKeyNextPhase()
    {
        return "PHASE_REFRESH_GO_NEXT";
    }

    public bool IsActionAllowed(CardSelectorSource source, CardSelectorAction action)
    {
        return action == CardSelectorAction.See;
    }

    public IPhase NextPhase()
    {
        return new DrawPhase();
    }

    public void OnPhaseEnded(PlayerArea playerArea)
    {
    }

    public void OnPhaseStarted(PlayerArea playerArea)
    {
        playerArea.Playmat.UnrestCostDeck();
        playerArea.Playmat.CharactersSlots.ForEach(x =>
        {
            if (x.Card.Rested)
            {
                x.Card.ToggleRest();
            }
        });

        if (playerArea.Playmat.LeaderSlotCard.Card.Rested)
        {
            playerArea.Playmat.LeaderSlotCard.Card.ToggleRest();
        }
    }
}
