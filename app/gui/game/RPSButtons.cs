using Godot;
using OPSProServer.Contracts.Models;
using System;

public partial class RPSButtons : Container
{
	public Button RockButton { get; private set; }
	public Button PaperButton { get; private set; }
	public Button ScissorsButton { get; private set; }
	public Button RandomButton { get; private set; }

	[Signal]
	public delegate void ClickRPSEventHandler(int rpsId);

	public override void _Ready()
	{
		RockButton = GetNode<Button>("HBoxContainer/Rock");
		PaperButton = GetNode<Button>("HBoxContainer/Paper");
		ScissorsButton = GetNode<Button>("HBoxContainer/Scissors");
		RandomButton = GetNode<Button>("Random");
	}

	public void SetDisabled(bool disabled)
    {
		RockButton.Disabled = disabled;
		PaperButton.Disabled = disabled;
		ScissorsButton.Disabled = disabled;
		RandomButton.Disabled = disabled;
    }

	public void OnClickRock()
    {
		EmitSignal(SignalName.ClickRPS, (int)RPSChoice.Rock);
    }

	public void OnClickPaper()
	{
		EmitSignal(SignalName.ClickRPS, (int)RPSChoice.Paper);
	}

	public void OnClickScissors()
	{
		EmitSignal(SignalName.ClickRPS, (int)RPSChoice.Scissors);
	}

	public void OnClickRandom()
	{
		var random = new Random();
		var min = (int)RPSChoice.Rock;
		var max = (int)RPSChoice.Scissors + 1;
		EmitSignal(SignalName.ClickRPS, random.Next(min, max));
	}
}
