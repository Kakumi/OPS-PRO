using Godot;
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
		EmitSignal(SignalName.ClickRPS, (int)OPSProServer.Contracts.Contracts.RockPaperScissors.Rock);
    }

	public void OnClickPaper()
	{
		EmitSignal(SignalName.ClickRPS, (int)OPSProServer.Contracts.Contracts.RockPaperScissors.Paper);
	}

	public void OnClickScissors()
	{
		EmitSignal(SignalName.ClickRPS, (int)OPSProServer.Contracts.Contracts.RockPaperScissors.Scissors);
	}

	public void OnClickRandom()
	{
		var random = new Random();
		var min = (int)OPSProServer.Contracts.Contracts.RockPaperScissors.Rock;
		var max = (int)OPSProServer.Contracts.Contracts.RockPaperScissors.Scissors + 1;
		EmitSignal(SignalName.ClickRPS, random.Next(min, max));
	}
}
