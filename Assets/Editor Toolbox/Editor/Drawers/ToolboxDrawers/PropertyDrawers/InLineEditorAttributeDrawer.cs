﻿using System.Collections.Generic;
#if !UNITY_2019_1_OR_NEWER
using System.Reflection;
#endif

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Toolbox.Editor.Drawers
{
    using Editor = UnityEditor.Editor;

    public class InLineEditorAttributeDrawer : ToolboxPropertyDrawer<InLineEditorAttribute>
    {
        /// <summary>
        /// Collection of all stored <see cref="Editor"/> instances.
        /// </summary>
        private static Dictionary<string, Editor> editorInstances = new Dictionary<string, Editor>();


        /// <summary>
        /// Clears and destroys particular editor mapped to the provided key.
        /// </summary>
        /// <param name="key"></param>
        private void ClearEditor(string key)
        {
            if (editorInstances.TryGetValue(key, out var editor))
            {
                editorInstances.Remove(key);
                Object.DestroyImmediate(editor);
            }
        }

        /// <summary>
        /// Clears and destroys all previously instantiated editor instances.
        /// </summary>
        private void ClearEditors()
        {
            foreach (var editor in editorInstances.Values)
            {
                Object.DestroyImmediate(editor);
            }

            editorInstances.Clear();
        }

        /// <summary>
        /// Draws the inlined version of the <see cref="Editor"></see> and handles all unexpected situations.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="attribute"></param>
        private void OnEditorGuiSafe(Editor editor, InLineEditorAttribute attribute)
        {
            if (!attribute.DrawHeader)
            {
                //force the expanded inspector if a header is not expected
                if (!InternalEditorUtility.GetIsInspectorExpanded(editor.target))
                {
                    InternalEditorUtility.SetIsInspectorExpanded(editor.target, true);
                    //NOTE: in older versions editor's foldouts are based on the m_IsVisible field and the Awake() method
#if !UNITY_2019_1_OR_NEWER
                    const string isVisibleFieldName = "m_IsVisible";
                    var isVisible = editor.GetType().GetField(isVisibleFieldName,
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    if (isVisible != null)
                    {
                        isVisible.SetValue(editor, true);
                    }
#endif
                }
            }

            //prevent custom editors for overriding label width
            var labelWidth = EditorGUIUtility.labelWidth;
            OnEditorGuiDraw(editor, attribute);
            EditorGUIUtility.labelWidth = labelWidth;
        }

        /// <summary>
        /// Draws the inlined version of the <see cref="Editor"></see>.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="attribute"></param>
        private void OnEditorGuiDraw(Editor editor, InLineEditorAttribute attribute)
        {
            //draw header if needed
            if (attribute.DrawHeader)
            {
                editor.DrawHeader();
            }

            //begin editor inside vertical group
            EditorGUILayout.BeginVertical(Style.inlinedStyle);
            EditorGUILayout.BeginVertical();

            //draw whole inspector and apply all changes 
            editor.serializedObject.Update();
            editor.OnInspectorGUI();
            editor.serializedObject.ApplyModifiedProperties();

            if (editor.HasPreviewGUI())
            {
                //draw preview if possible and needed
                if (attribute.DrawPreview)
                {
                    editor.OnPreviewGUI(EditorGUILayout.GetControlRect(false, attribute.PreviewHeight), Style.previewStyle);
                }

                if (attribute.DrawSettings)
                {
                    EditorGUILayout.BeginHorizontal();
                    editor.OnPreviewSettings();
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Handles the property drawing process and tries to create a inlined version of the <see cref="Editor"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, InLineEditorAttribute attribute)
        {
            var key = property.GetPropertyKey();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, label, property.isExpanded);
            if (EditorGUI.EndChangeCheck())
            {
                ClearEditor(key);
                return;
            }

            if (property.objectReferenceValue == null)
            {
                return;
            }

            //get (or create) editor for this property
            if (!editorInstances.TryGetValue(key, out var editor))
            {
                editorInstances[key] = editor = Editor.CreateEditor(property.objectReferenceValue);
            }

            if (property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, new GUIContent(property.objectReferenceValue.GetType().Name + " Inspector Preview"), true, Style.foldoutStyle))
            {                
                //draw and prewarm inlined editor   
                OnEditorGuiSafe(editor, attribute);
            }
        }


        /// <summary>
        /// Handles data clearing between editors.
        /// </summary>
        public override void OnGuiReload()
        {
            ClearEditors();
        }

        /// <summary>
        /// Checks if the provided property can be handled by this drawer.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }


        private static class Style
        {
            internal static readonly GUIStyle inlinedStyle;
            internal static readonly GUIStyle foldoutStyle;
            internal static readonly GUIStyle previewStyle;

            static Style()
            {
                inlinedStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    padding = new RectOffset(13, 12, 8, 8)
                };
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontSize = 9,
                    alignment = TextAnchor.MiddleLeft
                };

                previewStyle = new GUIStyle();
                previewStyle.normal.background = TextureUtility.CreatePersistantTexture(Color.clear);
            }
        }
    }
}