﻿using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class BeginHorizontalAttributeDrawer : ToolboxDecoratorDrawer<BeginHorizontalAttribute>
    {
        protected override void OnGuiBeginSafe(BeginHorizontalAttribute attribute)
        {
            var width = EditorGUIUtility.currentViewWidth;
            //set a new label & field width for this group
            EditorGUIUtility.labelWidth = width * attribute.LabelToWidthRatio;
            EditorGUIUtility.fieldWidth = width * attribute.FieldToWidthRatio;

            //begin horizontal group using internal utilities
            ToolboxLayoutHelper.BeginHorizontal();
        }
    }
}