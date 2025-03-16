using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MornNovel
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    internal sealed class ShowIfDrawer : PropertyDrawer
    {
        private bool TryGetBool(SerializedProperty property, out bool value)
        {
            var showIf = (ShowIfAttribute)attribute;
            var targetObject = property.serializedObject.targetObject;
            var propertyInfo = targetObject.GetType().GetProperty(
                showIf.PropertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo != null && propertyInfo.GetValue(targetObject) is bool boolValue)
            {
                value = boolValue;
                return true;
            }

            value = false;
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (TryGetBool(property, out var boolValue))
            {
                if (boolValue == true)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                EditorGUI.HelpBox(
                    position,
                    $"Property not found: {((ShowIfAttribute)attribute).PropertyName}",
                    MessageType.Error);
            }
        }
    }
}