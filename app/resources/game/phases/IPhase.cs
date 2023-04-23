using Godot;
using System;

public interface IPhase
{
    void OnPhaseStarted(PlayerArea playerArea);
    void OnPhaseEnded(PlayerArea playerArea);
    bool IsActionAllowed(CardSelectorSource source, CardSelectorAction action);
    IPhase NextPhase();
    string GetTrKeyNextPhase();
}
