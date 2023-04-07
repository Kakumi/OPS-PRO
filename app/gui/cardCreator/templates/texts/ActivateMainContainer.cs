using Godot;
using System;

public partial class ActivateMainContainer : TemplateCardEffect
{
    [Export]
    public string defaultSpace = ""; // |                             |

    [Export]
    public string spaceDon = ""; // |                  |

    public TextureRect DonTexture { get; private set; }
    public Label DonLabel { get; private set; }
    public Label DefaultText { get; private set; }

    private string _lastText = null;

    public override void _Ready()
    {
        DonTexture = GetNode<TextureRect>("Control/DonTexture");
        DonLabel = DonTexture.GetNode<Label>("Label");
        DefaultText = GetNode<Label>("DefaultText");

        DonTexture.Hide();
    }

    public override void UpdateDamage(int value)
    {

    }

    public override void UpdateDon(int value)
    {
        if (value == 0)
        {
            DonTexture.Hide();
            refreshText();
        } else
        {
            DonTexture.Show();
            DonLabel.Text = value.ToString();
            refreshText();
        }
    }

    public override void UpdateText(string text)
    {
        _lastText = text;
        refreshText();
    }

    private void refreshText()
    {
        var spaceDon = DonTexture.Visible ? this.spaceDon : "";
        DefaultText.Text = $"{defaultSpace}{spaceDon}{_lastText}";
    }
}
