using System.IO;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.Editor.Windows
{
	internal class ActOptions: EditorWindow
	{
		[UnityEditor.MenuItem("Window/Anti-Cheat Toolkit/Options")]
		private static void ShowWindow()
		{
			EditorWindow myself = GetWindow<ActOptions>(false, "ACT Options", true);
			myself.minSize = new Vector2(300, 100);
		}

		private void OnGUI()
		{
			GUILayout.Label("Injection Detector options", EditorStyles.boldLabel);

			bool enableInjectionDetector = false;

			BuildTarget bt = EditorUserBuildSettings.activeBuildTarget;


				enableInjectionDetector = EditorPrefs.GetBool(ActEditorGlobalStuff.PREFS_INJECTION_GLOBAL);
				enableInjectionDetector = GUILayout.Toggle(enableInjectionDetector, "Enable Injection Detector");

				if (GUILayout.Button("Edit Whitelist"))
				{
					ActAssembliesWhitelist.ShowWindow();
				}

				if (GUI.changed || EditorPrefs.GetBool(ActEditorGlobalStuff.PREFS_INJECTION) != enableInjectionDetector)
				{
					EditorPrefs.SetBool(ActEditorGlobalStuff.PREFS_INJECTION, enableInjectionDetector);
					EditorPrefs.SetBool(ActEditorGlobalStuff.PREFS_INJECTION_GLOBAL, enableInjectionDetector);
				}


			if (!enableInjectionDetector)
			{
				ActEditorGlobalStuff.CleanInjectionDetectorData();
			}
			else if (!File.Exists(ActEditorGlobalStuff.INJECTION_DATA_PATH))
			{
				ActPostprocessor.InjectionAssembliesScan();
			}
		}
	}
}