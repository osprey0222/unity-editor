﻿using System;

namespace UnityEngine
{
    /// <summary>
    /// Draws a information box if the associated value is null.
    /// Supported types: any <see cref="Object"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class NotNullAttribute : PropertyAttribute
    {
        public NotNullAttribute()
        {
            Label = "Variable has to be assigned.";
        }

        public NotNullAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }
    }
}