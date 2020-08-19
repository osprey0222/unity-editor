﻿using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(Vector2RangeAttribute))]
    public class Vector2RangeAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (Vector2RangeAttribute)base.attribute;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            if (EditorGUI.EndChangeCheck())
            {
                var vectorData = property.vector2Value;

                vectorData.x = Mathf.Clamp(vectorData.x, attribute.Min, attribute.Max);
                vectorData.y = Mathf.Clamp(vectorData.y, attribute.Min, attribute.Max);

                property.vector2Value = vectorData;
                property.serializedObject.ApplyModifiedProperties();
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Vector2;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.wideMode ? EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
