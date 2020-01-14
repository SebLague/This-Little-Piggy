using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InjectionDetector))]
public class InjectionDetectorEditor: Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUIStyle textStyle = new GUIStyle();
		textStyle.normal.textColor = GUI.skin.label.normal.textColor;
		textStyle.alignment = TextAnchor.UpperLeft;
#if UNITY_4_2
		textStyle.contentOffset = new Vector2(6, 0);
#else
		textStyle.contentOffset = new Vector2(2, 0);
#endif
		textStyle.wordWrap = true;

		EditorGUILayout.LabelField("Don't forget to start detection (check readme)!", textStyle);

		if (!EditorPrefs.GetBool(ActEditorGlobalStuff.PREFS_INJECTION_GLOBAL))
		{
			textStyle.normal.textColor = new Color32(220, 64, 64, 255);
			textStyle.fontStyle = FontStyle.Bold;

			EditorGUILayout.LabelField("Injection Detector is not enabled in ACT options (check readme)!", textStyle);
		}
		else if (!EditorPrefs.GetBool(ActEditorGlobalStuff.PREFS_INJECTION))
		{
			textStyle.normal.textColor = new Color32(220, 64, 64, 255);
			textStyle.fontStyle = FontStyle.Bold;

			EditorGUILayout.LabelField("Injection Detector disabled on current platform!", textStyle);
		}
	}
}