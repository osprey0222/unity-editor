﻿using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws a HelpBox within the Inspector Window above this field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HelpAttribute : ToolboxDecoratorAttribute
    {
        public HelpAttribute(string text, UnityMessageType type = UnityMessageType.Info)
        {
            Text = text;
            Type = type;
        }

        public UnityMessageType Type { get; private set; }

        public string Text { get; private set; }
    }

    public enum UnityMessageType
    {
        Noone,
        Info,
        Warning,
        Error
    }
}
