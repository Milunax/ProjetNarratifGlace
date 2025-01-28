using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(DirectionalPad))]
public class DirectionalPadEditor : Editor
{
    DirectionalPad PadTarget => (DirectionalPad)target;

    private MethodInfo[] _methods;

    private void OnEnable()
    {
        if (_methods == null)
        {
            _methods = PadTarget.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var idMethod = EditorGUILayout.Popup("Select Method", 0, _methods.Select(m => m.Name).ToArray());

        PadTarget.OnConfirm = (c) => _methods[idMethod].Invoke(PadTarget, new object[] { c });

        serializedObject.ApplyModifiedProperties();
    }
}
