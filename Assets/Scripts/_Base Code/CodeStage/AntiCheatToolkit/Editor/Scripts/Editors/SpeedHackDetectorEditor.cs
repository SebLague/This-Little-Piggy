using CodeStage.AntiCheat.Detectors;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpeedHackDetector))]
public class SpeedHackDetectorEditor : Editor
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
	}
}
