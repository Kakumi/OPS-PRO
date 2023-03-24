using DiscordRPC.Logging;
using Godot;
using System;

public partial class DiscordLogger : Node, ILogger
{
    public LogLevel Level { get; set; }

    public void Error(string message, params object[] args)
    {
        Serilog.Log.Error($"Discord RPC {message}", args);
    }

    public void Info(string message, params object[] args)
    {
        Serilog.Log.Information($"Discord RPC {message}", args);
    }

    public void Trace(string message, params object[] args)
    {
        Serilog.Log.Information($"Discord RPC {message}", args);
    }

    public void Warning(string message, params object[] args)
    {
        Serilog.Log.Warning($"Discord RPC {message}", args);
    }
}
