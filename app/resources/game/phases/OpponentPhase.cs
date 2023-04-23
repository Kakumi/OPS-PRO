﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OpponentPhase : IPhase
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
        return null;
    }

    public void OnPhaseEnded(PlayerArea playerArea)
    {
    }

    public void OnPhaseStarted(PlayerArea playerArea)
    {
    }
}
