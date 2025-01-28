using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;
using UnityEditor.UIElements;
using UnityEngine.UI;
using System.Reflection.Emit;
using UnityEngine.UIElements;
public class DialogueEditorWindow : EditorWindow
{
    private DialogueContainerSO currentDialogueContainer;
    private DialogueGraphView graphview;
    private DialogueSaveAndLoad saveAndLoad;
    private UnityEngine.UIElements.Label nameOfDialoguecontainer;

    public LanguageType LanguageType { get; internal set; }

    [OnOpenAsset(1)]
    public static bool ShowWindow(int _instanceId, int line)
    {
        UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceId);
        if (item is DialogueContainerSO)
        {
            DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));
            window.titleContent = new GUIContent("Dialogue Editor");
            window.currentDialogueContainer = item as DialogueContainerSO; // Reads it as type DialogueContainerSO
            window.minSize = new Vector2(500, 250);
            window.Load();
        }
        return false;
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolBar();
        Load();
    }
    private void OnDisable()
    {
        rootVisualElement.Remove(graphview);   
    }

    private void ConstructGraphView()
    {
        graphview = new DialogueGraphView(this);
        graphview.StretchToParentSize();
        rootVisualElement.Add(graphview);
        saveAndLoad = new DialogueSaveAndLoad(graphview);
    }
                                            
    private void GenerateToolBar()          
    {
      
        StyleSheet styleSheet = Resources.Load<StyleSheet>("GraphViewStyleSheet");
        rootVisualElement.styleSheets.Add(styleSheet);

        Toolbar toolbar = new Toolbar();

        // Save button
        UnityEngine.UIElements.Button saveBtn = new UnityEngine.UIElements.Button()
        {
            text = "Save"
        };
        saveBtn.clicked += () =>
        {
            Save();
        };
        toolbar.Add(saveBtn);
        // Load Button
        UnityEngine.UIElements.Button loadBtn = new UnityEngine.UIElements.Button()
        {
            text = "Load"
        };
        loadBtn.clicked += () =>
        {
            Load();
        };
        toolbar.Add(loadBtn);
        
        //ToolbarMenu toolbarMenu = new ToolbarMenu(); (1.1, 17.30)

        //Name of current DialogueContainer you have open
        nameOfDialoguecontainer = new UnityEngine.UIElements.Label("");
        toolbar.Add(nameOfDialoguecontainer);
        nameOfDialoguecontainer.AddToClassList("nameOfDialoguecontainer");
        rootVisualElement.Add(toolbar);
    }

    private void Save()
    {
        if(currentDialogueContainer != null)
        {
            saveAndLoad.Save(currentDialogueContainer);
        }
    }
    private void Load()
    {
        if (currentDialogueContainer != null)
        {
            nameOfDialoguecontainer.text = "Name:   " + currentDialogueContainer.name;
            saveAndLoad.Load(currentDialogueContainer);
        }

    }
}
