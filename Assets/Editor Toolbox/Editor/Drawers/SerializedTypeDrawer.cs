﻿using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ClassTypeConstraintAttribute), true)]
    [CustomPropertyDrawer(typeof(SerializedType))]
    public sealed class SerializedTypeDrawer : ToolboxNativePropertyDrawer
    {
        /// <summary>
        /// Creates formatted type name depending on <see cref="ClassGrouping"/> value.
        /// </summary>
        /// <param name="type">Type to display.</param>
        /// <param name="grouping">Format grouping type.</param>
        /// <returns></returns>
        private static string FormatGroupedTypeName(Type type, ClassGrouping grouping)
        {
            var name = type.FullName;

            switch (grouping)
            {
                default:
                case ClassGrouping.None:
                    return name;

                case ClassGrouping.ByNamespace:
                    return name.Replace('.', '/');

                case ClassGrouping.ByNamespaceFlat:
                    var lastPeriodIndex = name.LastIndexOf('.');
                    if (lastPeriodIndex != -1)
                    {
                        name = name.Substring(0, lastPeriodIndex) + "/" + name.Substring(lastPeriodIndex + 1);
                    }
                    return name;

                case ClassGrouping.ByAddComponentMenu:
                    var addComponentMenuAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
                    if (addComponentMenuAttributes.Length == 1)
                    {
                        return ((AddComponentMenu)addComponentMenuAttributes[0]).componentMenu;
                    }

                    return "Scripts/" + type.FullName.Replace('.', '/');
            }
        }


        /// <summary>
        /// Dictionary used to store all previously filtered types.
        /// </summary>
        private readonly static Dictionary<Type, List<Type>> filteredTypes = new Dictionary<Type, List<Type>>();


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var refAttribute = Attribute;
            var refProperty = property.FindPropertyRelative("classReference");

            //validate serialized data
            if (refAttribute == null || refAttribute.AssemblyType == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var refType = !string.IsNullOrEmpty(refProperty.stringValue) ? Type.GetType(refProperty.stringValue) : null;
            var refTypes = new List<Type>();
            var refLabels = new List<string>() { "<None>" };
            var index = -1;

            //get stored types if possible or create new item
            if (!filteredTypes.TryGetValue(refAttribute.AssemblyType, out refTypes))
            {
                filteredTypes[refAttribute.AssemblyType] = refTypes = refAttribute.GetFilteredTypes();
            }

            //create labels from filtered types
            for (int i = 0; i < refTypes.Count; i++)
            {
                var menuType = refTypes[i];
                var menuLabel = FormatGroupedTypeName(menuType, refAttribute.Grouping);

                if (menuType == refType)
                {
                    index = i;
                }

                refLabels.Add(menuLabel);
            }

            //draw the reference property
            label = EditorGUI.BeginProperty(position, label, property);
            label = property.name != "data" ? label : GUIContent.none;
            //draw the proper label field
            position = EditorGUI.PrefixLabel(position, label);

            index = EditorGUI.Popup(position, index + 1, refLabels.ToArray());
            //get the correct class reference, index = 0 is reserved to <None> type
            refProperty.stringValue = index >= 1 ? SerializedType.GetClassReference(refTypes[index - 1]) : "";
            EditorGUI.EndProperty();
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.type == nameof(SerializedType);
        }


        private ClassTypeConstraintAttribute Attribute => attribute as ClassTypeConstraintAttribute;
    }
}