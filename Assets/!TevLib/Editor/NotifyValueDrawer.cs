using System.Collections;
using System.Reflection;
using _TevLib.CoreLib;
using UnityEditor;
using UnityEngine;

namespace _TevLib.Editor
{
    [CustomPropertyDrawer(typeof(NotifyValue<>), true)]
    internal sealed class NotifyValueDrawer : PropertyDrawer
    {
        private const string SerializedValueName = "value";
        private const string ValuePropertyName = "Value";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative(SerializedValueName);
            if (valueProperty == null)
            {
                EditorGUI.HelpBox(position, "NotifyValue has no serialized value.", MessageType.Error);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, valueProperty, label, true);
            bool changed = EditorGUI.EndChangeCheck();
            EditorGUI.EndProperty();

            if (!changed || !Application.isPlaying)
                return;

            object nextValue = valueProperty.boxedValue;
            foreach (Object targetObject in property.serializedObject.targetObjects)
            {
                object notifyValue = GetPathValue(targetObject, property.propertyPath);
                PropertyInfo valueSetter = notifyValue?.GetType().GetProperty(
                    ValuePropertyName,
                    BindingFlags.Instance | BindingFlags.Public);

                valueSetter?.SetValue(notifyValue, nextValue);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty = property.FindPropertyRelative(SerializedValueName);
            return valueProperty == null
                ? EditorGUIUtility.singleLineHeight * 2f
                : EditorGUI.GetPropertyHeight(valueProperty, label, true);
        }

        private static object GetPathValue(object source, string propertyPath)
        {
            string path = propertyPath.Replace(".Array.data[", "[");
            string[] elements = path.Split('.');

            foreach (string element in elements)
            {
                int bracketIndex = element.IndexOf('[');
                if (bracketIndex < 0)
                {
                    source = GetMemberValue(source, element);
                    continue;
                }

                string memberName = element.Substring(0, bracketIndex);
                int index = int.Parse(element.Substring(
                    bracketIndex + 1,
                    element.Length - bracketIndex - 2));
                source = GetIndexedValue(source, memberName, index);
            }

            return source;
        }

        private static object GetMemberValue(object source, string memberName)
        {
            if (source == null)
                return null;

            const BindingFlags flags = BindingFlags.Instance
                                       | BindingFlags.Public
                                       | BindingFlags.NonPublic;

            for (System.Type type = source.GetType(); type != null; type = type.BaseType)
            {
                FieldInfo field = type.GetField(memberName, flags);
                if (field != null)
                    return field.GetValue(source);

                PropertyInfo property = type.GetProperty(memberName, flags);
                if (property != null)
                    return property.GetValue(source);
            }

            return null;
        }

        private static object GetIndexedValue(object source, string memberName, int index)
        {
            IEnumerable collection = GetMemberValue(source, memberName) as IEnumerable;
            if (collection == null)
                return null;

            IEnumerator enumerator = collection.GetEnumerator();
            for (int currentIndex = 0; currentIndex <= index; currentIndex++)
            {
                if (!enumerator.MoveNext())
                    return null;
            }

            return enumerator.Current;
        }
    }
}
