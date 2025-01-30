using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

using UnityEngine;

//[CustomEditor(typeof(DirectionalPadButton))]
public class DirectionalPadButtonEditor : Editor
{
    DirectionalPadButton PadTarget => (DirectionalPadButton)target;

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
        EditorGUI.BeginChangeCheck();

        //PadTarget.selectedMethodId = EditorGUILayout.Popup("Select Method", PadTarget.selectedMethodId, _methods.Select(m => m.Name).ToArray());

        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
    }
}
