﻿using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class LineAttributeDrawer : ToolboxDecoratorDrawer<LineAttribute>
    {
        protected override void OnGuiBeginSafe(LineAttribute attribute)
        {
            ToolboxEditorGui.DrawLayoutLine(attribute.Thickness, attribute.Padding, attribute.GetLineColor());
        }
    }
}