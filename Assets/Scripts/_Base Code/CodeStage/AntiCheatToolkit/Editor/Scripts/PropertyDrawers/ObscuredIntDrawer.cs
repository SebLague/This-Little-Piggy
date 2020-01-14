using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredInt))]
	public class ObscuredIntDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SerializedProperty cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			SerializedProperty fakeValue = prop.FindPropertyRelative("fakeValue");
			SerializedProperty inited = prop.FindPropertyRelative("inited");

			int currentCryptoKey = cryptoKey.intValue;
			int val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.intValue = 444444;
				}
				hiddenValue.intValue = ObscuredInt.Encrypt(0, currentCryptoKey);
				inited.boolValue = true;
			}
			else
			{
				val = ObscuredInt.Decrypt(hiddenValue.intValue, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.IntField(position, label, val);
			if (EditorGUI.EndChangeCheck())
				hiddenValue.intValue = ObscuredInt.Encrypt(val, currentCryptoKey);

			fakeValue.intValue = val;
		}
	}
}