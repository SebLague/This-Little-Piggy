using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.Design;

/*
 * Provides CustomEditor initialization code for given script. 
 */


public class EditorParameterGenerator : EditorWindow {
	
	// Modify this type to change type of scripts that are returned
	private Type searchType = typeof(MonoBehaviour);
	
	// System
	private MonoScript[] allScripts;
	private MonoScript targetScript;

	private bool includePrivateVars;
	private string currentSelectedTypeName = "NONE";
	private string generatedScript;
	
	private bool showGeneratedScript;
	private bool scriptSelectionFoldout;
	
	// Create window
    [MenuItem("Tools/Custom Editor/Generate Parameters")]
    public static void ShowWindow() {
        EditorWindow.GetWindowWithRect(typeof(EditorParameterGenerator), new Rect(0,0,450,800));
    }
    
	// Draw GUI
	void OnGUI() {
		includePrivateVars = EditorGUILayout.Toggle("Include private vars ",includePrivateVars);
		scriptSelectionFoldout = EditorGUILayout.Foldout(scriptSelectionFoldout, "Selected Type: " + currentSelectedTypeName, EditorStyles.radioButton);
		
		// Display list of scripts in project
		if (scriptSelectionFoldout && allScripts != null) {
			for (int i = 0; i < allScripts.Length; i ++) {
				if (GUILayout.Button(allScripts[i].name)) {
					scriptSelectionFoldout = false;
					targetScript = allScripts[i];
					currentSelectedTypeName = targetScript.name;
					GenerateInitCode();
				}
			}
		}
		EditorGUILayout.Space();
		if (GUILayout.Button("Refresh")) {
			GetScriptsList();
			GenerateInitCode();
		}
		
		// Show generated text
		if (showGeneratedScript) {
			EditorGUILayout.TextArea(generatedScript);	
		}
    }
    
	// Get a list of all scripts of type *searchType*
	public void GetScriptsList() {
		MonoScript[] scripts = (MonoScript[])UnityEngine.Object.FindObjectsOfTypeIncludingAssets(typeof(MonoScript));
 		List<MonoScript> result = new List<MonoScript>();
 		
		foreach( MonoScript script in scripts )	{
            if (script.GetClass() != null && script.GetClass().IsSubclassOf(searchType))
				result.Add(script);
		}
		allScripts = result.ToArray();
	}
	

	void OnFocus () {
		GenerateInitCode();
		scriptSelectionFoldout = true;
		GetScriptsList();
	}
	
	void OnDestroy() {
		targetScript = null;
		currentSelectedTypeName = "NONE";
	}
	
	// Generate init code from target script
	private void GenerateInitCode() {
		Type classType = null;
		if (targetScript)
			classType = Type.GetType(targetScript.name+", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
		
		if (classType != null) {
			showGeneratedScript = true;
			
			// Fetch varaibls from script
			FieldInfo[] classInfo = new FieldInfo[0];
			if (includePrivateVars) {
				classInfo = classType.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			else {
				classInfo = classType.GetFields();
			}
			
			// Write code
			string outputString = "#region Initialize parameters";
			foreach (FieldInfo info in classInfo) {
				outputString += "\n\tprivate SerializedProperty " + info.Name + ";";
			}
		
			outputString += "\n\n\tprotected override void Initialize () {";
			foreach (FieldInfo info in classInfo) {
				outputString += "\n\t\t" + info.Name + " = serializedObject.FindProperty(\"" + info.Name + "\");";
			}
			outputString += "\n\t}\n#endregion\n";
			outputString += "\n\n\tpublic override void OnInspectorGUI () {\n\t\tStartEdit();\n\t\tif (Section(\"New Section\")) {";
			
			foreach (FieldInfo info in classInfo) {
				outputString += "\n\t\t\tPropertyField(\"" + info.Name + "\", \"\", " + info.Name + ");";
			}
			outputString += "\n\t\t}\n\t\tEndEdit();\n\t}";
			generatedScript = outputString;
		}
	}

}

