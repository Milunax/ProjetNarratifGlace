
#if !TranslationSystemImplemented 

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImplementationTranslationSystemReference : MonoBehaviour
{
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            AddNodeSystemScriptingDefine();
        }
    }

    static void AddNodeSystemScriptingDefine()
    {
        string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (!scriptingDefineSymbols.Contains("TranslationSystemImplemented"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, scriptingDefineSymbols + ";TranslationSystemImplemented");
        }
    }
}

#endif
