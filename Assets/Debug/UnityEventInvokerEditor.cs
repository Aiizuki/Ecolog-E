using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(MonoBehaviour), true)]
public class UnityEventInvokerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FieldInfo[] fields = target.GetType().GetFields(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        bool hasEvents = false;

        foreach (FieldInfo field in fields)
        {
            if (!typeof(UnityEventBase).IsAssignableFrom(field.FieldType))
                continue;

            UnityEventBase unityEvent = (UnityEventBase)field.GetValue(target);
            if (unityEvent == null)
                continue;

            if (!hasEvents)
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.LabelField("Invoke Events", EditorStyles.boldLabel);
                hasEvents = true;
            }

            if (GUILayout.Button($"▶  {field.Name}"))
            {
                MethodInfo invoke = field.FieldType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
                invoke?.Invoke(unityEvent, null);
            }
        }
    }
}