﻿using System;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    internal static class ToolboxEditorLog
    {
        private const string tag = "Editor Toolbox";
        private const string format = "[{0}] {1}";


        private static string GetPropertyLocation(SerializedProperty property)
        {
            return property.name + " property in " + property.serializedObject.targetObject;
        }


        internal static void AttributeUsageWarning(Attribute attribute, string message)
        {
            LogWarning(attribute.GetType().Name + ": " + message);
        }

        internal static void AttributeUsageWarning(Attribute attribute, SerializedProperty property, string message)
        {
            LogWarning(attribute.GetType().Name + ": " + GetPropertyLocation(property) + ": " + message);
        }

        internal static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property)
        {
            AttributeUsageWarning(attribute, property, "Associated attribute cannot be used on this property.");
        }

        internal static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property, Type targetType)
        {
            WrongAttributeUsageWarning(attribute, property, targetType.ToString());
        }

        internal static void WrongAttributeUsageWarning(Attribute attribute, SerializedProperty property, string targetType)
        {
            AttributeUsageWarning(attribute, property, "Associated attribute can be used only on " + targetType + " type properties.");
        }

        internal static void AttributeNotSupportedWarning(Attribute attribute)
        {
            AttributeNotSupportedWarning(attribute.GetType());
        }

        internal static void AttributeNotSupportedWarning(Type attributeType)
        {
            LogWarning(attributeType + " is not supported. Assign it in the " + nameof(ToolboxEditorSettings) + ".");
        }

        internal static void PropertyNotFoundWarning(SerializedProperty property, string propertyName)
        {
            LogWarning(GetPropertyLocation(property) + ": Property " + propertyName + " not found.");
        }

        internal static void TypeNotSupportedWarning(SerializedProperty property, Type type)
        {
            TypeNotSupportedWarning(property, type.Name);
        }

        internal static void TypeNotSupportedWarning(SerializedProperty property, string type)
        {
            LogWarning(GetPropertyLocation(property) + ": " + type + " value type is not supported in comparison.");
        }

        internal static void PropertyLocation(SerializedProperty property)
        {
            LogMessage(GetPropertyLocation(property));
        }

        internal static void PrefabExpectedWarning()
        {
            PrefabExpectedWarning(null);
        }

        internal static void PrefabExpectedWarning(Object referenceObject)
        {
            var name = referenceObject ? referenceObject.name : "object";
            LogWarning(name + " should be a prefab.");
        }

        internal static void KitInitializationMessage()
        {
            LogWarning("Settings initialization needed. Go to <b>Edit/Project Settings.../Editor Toolbox</b> and find an existing Settings file ('Refresh') or create new.");
        }


        internal static void LogMessage(string message)
        {
            Debug.LogFormat(format, tag, message);
        }

        internal static void LogWarning(string message)
        {
            Debug.LogWarningFormat(format, tag, message);
        }


        internal static void LogError(string message)
        {
            Debug.LogErrorFormat(format, tag, message);
        }
    }
}