using Godot;
using System;
using System.Threading.Tasks;

public interface IPhase
{
    Task OnPhaseStarted(PlayerArea playerArea);
    Task OnPhaseEnded(PlayerArea playerArea);
    bool IsActionAllowed(CardSelectorSource source, CardSelectorAction action);
    IPhase NextPhase();
    string GetTrKeyNextPhase();
    bool IsAutoNextPhase();
}
