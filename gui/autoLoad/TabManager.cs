using Godot;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;

public class TabManager : Node, ILogEventSink
{
    private static TabManager _instance;
    private Dictionary<TabType, Queue<string>> _textToAdd;

    public override void _Ready()
    {
        _instance = this;

        _textToAdd = new Dictionary<TabType, Queue<string>>();
    }

    public static TabManager Instance => _instance;

    public void Add(TabType tabType, string text)
    {
        var tabs = GetTree().CurrentScene.GetChildren().SearchOne<Tabs>();
        if (tabs == null)
        {
            if (!_textToAdd.ContainsKey(tabType))
            {
                _textToAdd.Add(tabType, new Queue<string>());
            }

            _textToAdd[tabType].Enqueue(text);
            if (_textToAdd[tabType].Count > 500)
            {
                _textToAdd[tabType].Dequeue();
            }
        } else
        {
            AddUnaddedText(tabs);

            Write(tabType, tabs, text);
        }
    }

    private void Write(TabType tabType, Tabs tabs, string text)
    {
        switch (tabType)
        {
            case TabType.Chat:
                tabs.Chat.Add(text);
                break;
            case TabType.Log:
                tabs.Logs.Add(text);
                break;
        }
    }

    public void AddUnaddedText(Tabs tabs)
    {
        foreach(var entry in _textToAdd) {
            for(int i = 0; i < entry.Value.Count; i++)
            {
                Write(entry.Key, tabs, entry.Value.Dequeue());
            }
        }
    }

    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.RenderMessage();
        Add(TabType.Log, message);
    }
}
