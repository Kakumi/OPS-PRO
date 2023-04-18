using Godot;
using System;

public partial class SlotCardSelector : Container
{
	private CardSelectorSource _source;
	[Export]
	public CardSelectorSource Source
    {
		get => _source;
		set
        {
			_source = value;
			if (SourceLabel != null)
            {
				SourceLabel.Text = Tr(Source.GetTrKey());
			}
        }
    }

	private bool _showSource;
	[Export]
	public bool ShowSource
	{
		get => _showSource;
		set
		{
			_showSource = value;
			if (SourceLabel != null)
			{
				SourceLabel.Visible = value;
			}
		}
	}

	public Guid TargetGuid { get; set; }

    public Label SourceLabel { get; private set; }
    public SlotCard SlotCard { get; private set; }

    public override void _Ready()
	{
		SourceLabel = GetNode<Label>("SourceLabel");
		SlotCard = GetNode<SlotCard>("SlotCard");

		SourceLabel.Text = Tr(Source.GetTrKey());
		SourceLabel.Visible = ShowSource;
	}
}
