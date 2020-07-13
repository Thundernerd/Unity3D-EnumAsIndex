using System;
using System.Collections;
using TNRD.Constraints;
using UnityEditor;
using UnityEngine;

namespace TNRD.Utilities
{
    [CustomPropertyDrawer(typeof(EnumAsIndexAttribute))]
    public class EnumAsIndexPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enumAsIndexAttribute = (EnumAsIndexAttribute) attribute;
            var enumType = enumAsIndexAttribute.Type;
            var names = Enum.GetNames(enumType);

            EditorGUI.BeginChangeCheck();
            EnsureSize(property, names.Length);
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript();
            }

            EditorGUI.BeginProperty(position, label, property);

            var labelRect = Constrain.To(position)
                .Width.Absolute(EditorGUIUtility.labelWidth)
                .ToRect();

            var contentRect = Constrain.To(position)
                .Left.Relative(EditorGUIUtility.labelWidth)
                .ToRect();

            int index = Convert.ToInt32(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.Popup(labelRect, index, names);
            EditorGUI.PropertyField(contentRect, property, GUIContent.none);

            EditorGUI.EndProperty();
        }

        private void EnsureSize(SerializedProperty property, int requiredSize)
        {
            var value = fieldInfo.GetValue(property.serializedObject.targetObject);
            if (value is Array source)
            {
                ResizeArray(property, requiredSize, source);
            }
            else if (value is IList list)
            {
                ResizeList(requiredSize, list);
            }
        }

        private void ResizeArray(SerializedProperty property, int requiredSize, Array source)
        {
            var destination = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), requiredSize);
            Array.Copy(source, destination, Mathf.Min(source.Length, requiredSize));
            fieldInfo.SetValue(property.serializedObject.targetObject, destination);
        }

        private void ResizeList(int requiredSize, IList list)
        {
            var delta = requiredSize - list.Count;
            if (delta > 0)
            {
                for (int i = 0; i < delta; i++)
                {
                    list.Add(Activator.CreateInstance(fieldInfo.FieldType.GenericTypeArguments[0]));
                }
            }
            else if (delta < 0)
            {
                var count = Mathf.Abs(delta);
                for (int i = list.Count - 1; i >= count; i--)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}
