using Godot;
using Serilog.Core;
using Serilog.Events;
using System;

public class GodotSink : ILogEventSink
{
    private IFormatProvider _formatProvider;

    public GodotSink(IFormatProvider formatProvider)
    {
        _formatProvider = formatProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.RenderMessage(_formatProvider);
        GD.Print(DateTimeOffset.Now.ToString() + " " + message);
    }
}
