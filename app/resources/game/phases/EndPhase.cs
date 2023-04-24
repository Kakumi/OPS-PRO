using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EndPhase : IPhase
{
    public string GetTrKeyNextPhase()
    {
        return "PHASE_END_GO_NEXT";
    }

    public bool IsActionAllowed(CardSelectorSource source, CardSelectorAction action)
    {
        return action == CardSelectorAction.See;
    }

    public IPhase NextPhase()
    {
        return new RefreshPhase(); //new OpponentPhase();
    }

    public void OnPhaseEnded(PlayerArea playerArea)
    {
    }

    public void OnPhaseStarted(PlayerArea playerArea)
    {
        playerArea.UpdatePhase(NextPhase());
        //PlayerArea opponentArea;
        //if (playerArea.Gameboard.PlayerArea == playerArea)
        //{
        //    opponentArea = playerArea.Gameboard.OpponentArea;
        //} else
        //{
        //    opponentArea = playerArea.Gameboard.PlayerArea;
        //}

        //opponentArea.UpdatePhase(new DrawPhase());
    }
}