using Godot;
using System;
using System.Linq;

public partial class SecurityLoad : Node
{
    public override void _Ready()
    {
        if (!OS.IsDebugBuild())
        {
            var args = OS.GetCmdlineArgs().ToList<string>();
            if (!args.Any(x => x == "Wg9*8Z49UYvU&yU@@F"))
            {
                GetTree().Quit();
            }
        }
    }
}
