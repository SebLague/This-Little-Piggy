using CodeStage.AntiCheat.Detectors;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObscuredCheatingDetector))]
public class ObscuredCheatingDetectorEditor : Editor
{
	private SerializedProperty floatEpsilon;
	private SerializedProperty vector2Epsilon;
	private SerializedProperty vector3Epsilon;
	private SerializedProperty quaternionEpsilon;
	public void OnEnable()
	{
		floatEpsilon = serializedObject.FindProperty("floatEpsilon");
		vector2Epsilon = serializedObject.FindProperty("vector2Epsilon");
		vector3Epsilon = serializedObject.FindProperty("vector3Epsilon");
		quaternionEpsilon = serializedObject.FindProperty("quaternionEpsilon");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		base.DrawDefaultInspector();
		EditorGUILayout.PropertyField(floatEpsilon, new GUIContent("Float Epsilon", "Max allowed difference between encrypted and fake values in ObscuredFloat. Increase in case of false positives."));
		EditorGUILayout.PropertyField(vector2Epsilon, new GUIContent("Vector2 Epsilon", "Max allowed difference between encrypted and fake values in ObscuredVector2. Increase in case of false positives."));
		EditorGUILayout.PropertyField(vector3Epsilon, new GUIContent("Vector3 Epsilon", "Max allowed difference between encrypted and fake values in ObscuredVector3. Increase in case of false positives."));
		EditorGUILayout.PropertyField(quaternionEpsilon, new GUIContent("Quaternion Epsilon", "Max allowed difference between encrypted and fake values in ObscuredQuaternion. Increase in case of false positives."));

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
