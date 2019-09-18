﻿using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    public static class ToolboxEditorGui
    {
        /// <summary>
        /// Creates inspector line.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="padding"></param>
        public static void DrawLine(Rect rect, float thickness = 0.75f, float padding = 6.0f)
        {
            rect.y += padding / 2;
            rect.height = thickness;

            EditorGUI.DrawRect(rect, Style.standardLineColor);
        }

        /// <summary>
        /// Creates inspector line using <see cref="GUILayout"/> class.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="padding"></param>
        public static void DrawLayoutLine(float thickness = 0.75f, float padding = 6.0f)
        {
            DrawLine(EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness)), thickness, padding);
        }

        /// <summary>
        /// Creates asset preview using provided asset object.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name=""></param>
        public static void DrawLayoutAssetPreview(UnityEngine.Object asset, float width = 64, float height = 64)
        {
            var previewTexture = AssetPreview.GetAssetPreview(asset);

            if (!previewTexture) return;

            var style = new GUIStyle();
            style.normal.background = previewTexture;
            width = Mathf.Clamp(width, 0, previewTexture.width);
            height = Mathf.Clamp(height, 0, previewTexture.height);
            var previewOptions = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(width),
                GUILayout.MaxHeight(height)
            };

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(Style.boxStyle);
            EditorGUI.LabelField(EditorGUILayout.GetControlRect(true, height, previewOptions), GUIContent.none, style);
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using standard background.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ReorderableList CreateRoundList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            return new ReorderableList(property, elementLabel, draggable, true, fixedSize);
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using non-standard(box style) background.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ReorderableList CreateBoxedList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            return new ReorderableList(property, elementLabel, draggable, true, fixedSize)
            {
                drawHeaderBackgroundCallback = (Rect rect) =>
                {
                    rect.y += EditorGUIUtility.standardVerticalSpacing / 2;
                    if (Event.current.type == EventType.Repaint)
                    {
                        Style.boxStyle.Draw(rect, false, false, false, false);
                    }
                },
                drawMiddleBackgroundCallback = (Rect rect) =>
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        Style.boxStyle.Draw(rect, false, false, false, false);
                    }
                },
                drawFooterBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= EditorGUIUtility.standardVerticalSpacing / 2;
                    if (Event.current.type == EventType.Repaint)
                    {
                        Style.boxStyle.Draw(rect, false, false, false, false);
                    }
                },
                FooterHeight = 15
            };
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using non-standard(lined style) background.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ReorderableList CreateLinedList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            return new ReorderableList(property, elementLabel, draggable, true, fixedSize)
            {
                drawHeaderBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= Style.spacing;
                    DrawLine(rect);
                },
                drawMiddleBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= Style.spacing * 2;
                    DrawLine(rect);
                },
                drawFooterBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= Style.spacing * 2 - rect.height;
                    DrawLine(rect);
                },
                FooterHeight = 14
            };
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> with provided <see cref="ListStyle"/> type.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ReorderableList CreateList(SerializedProperty property, ListStyle style, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            switch (style)
            {
                case ListStyle.Boxed: return CreateBoxedList(property, elementLabel, fixedSize, draggable);
                case ListStyle.Lined: return CreateLinedList(property, elementLabel, fixedSize, draggable);
                case ListStyle.Round: return CreateRoundList(property, elementLabel, fixedSize, draggable);
                default: return null;
            }
        }


        /// <summary>
        /// Custom style representation.
        /// </summary>
        internal static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal static readonly Color standardLineColor = new Color(0.3f, 0.3f, 0.3f);
            internal static readonly Color standardBackgroundColor = new Color(0.82f, 0.82f, 0.82f);

            internal static GUIStyle boxStyle;

            static Style()
            {
                boxStyle = new GUIStyle(GUI.skin.box);
            }
        }
    }
}