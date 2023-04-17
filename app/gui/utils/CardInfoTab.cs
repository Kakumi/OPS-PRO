using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class CardInfoTab : TabInfo
{
	protected RichTextLabel InfoText { get; set; }
	protected RichTextLabel EffectsText { get; set; }

	public override void _Ready()
	{
		base._Ready();
		InfoText = GetNode<RichTextLabel>("MarginContainer/ScrollContainer/Texts/InfoText");
		EffectsText = GetNode<RichTextLabel>("MarginContainer/ScrollContainer/Texts/EffectsText");
	}

	public void ShowcardResource(CardResource cardResource)
	{
		if (cardResource != null)
        {
			string info = Tr("INFOTAB_CARDINFO");

			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("{card_type}", cardResource.CardType);
			dic.Add("{name}", cardResource.Name);
			dic.Add("{attribute}", cardResource.Attribute);
			dic.Add("{cost_text}", cardResource.CardType == "LEADER" ? Tr("CARDINFO_LIFE") : Tr("CARDINFO_COST"));
			dic.Add("{cost}", cardResource.Cost + "");
			dic.Add("{power}", cardResource.Power + "");
			dic.Add("{colors}", string.Join("/", cardResource.Colors));
			dic.Add("{types}", string.Join("/", cardResource.Types));
			dic.Add("{set}", cardResource.Set);

			foreach (var entry in dic)
			{
				info = info.Replace(entry.Key, entry.Value);
			}

			InfoText.Text = info;

			var effects = cardResource.Effects.Select(x => Regex.Replace(x, @"\[(.*?)\]", "[b][$1][/b]"));
			EffectsText.Text = string.Join("\n", effects);
		} else
        {
			InfoText.Text = null;
			EffectsText.Text = null;
		}
	}
}
