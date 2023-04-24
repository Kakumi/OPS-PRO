using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MainPhase : IPhase
{
    public string GetTrKeyNextPhase()
    {
        return "PHASE_MAIN_GO_NEXT";
    }

    public bool IsActionAllowed(CardSelectorSource source, CardSelectorAction action)
    {
        return true;
    }

    public IPhase NextPhase()
    {
        return new EndPhase();
    }

    public Task OnPhaseEnded(PlayerArea playerArea)
    {
        return Task.CompletedTask;
    }

    public Task OnPhaseStarted(PlayerArea playerArea)
    {
        return Task.CompletedTask;
    }

    public bool IsAutoNextPhase()
    {
        return false;
    }
}