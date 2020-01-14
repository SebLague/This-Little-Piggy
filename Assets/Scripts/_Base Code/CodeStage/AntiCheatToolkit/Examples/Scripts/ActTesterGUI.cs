using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ActTesterGUI : MonoBehaviour
{
	public ObscuredVector3Test obscuredVector3Test;
	public ObscuredFloatTest obscuredFloatTest;
	public ObscuredIntTest obscuredIntTest;
	public ObscuredStringTest obscuredStringTest;
	public ObscuredPrefsTest obscuredPrefsTest;

	public ObscuredInt dummyObscuredInt = 1234;
	public ObscuredFloat dummyObscuredFloat = 5678f;
	public ObscuredString dummyObscuredString = "dummy obscured string";
	public ObscuredBool dummyObscuredBool = true;

	private bool savesAlterationDetected = false;

#if !UNITY_FLASH
	private int savesLock = 0;
	private bool foreignSavesDetected = false;
#endif

	private DetectorsUsageExample detectorsUsageExample;

	private void Awake()
	{
		// Since we already have SpeedHackDetector in our scene, we may use settings we set 
		// there in inspector. Just avoid them while calling StartDetection, like this:
		// SpeedHackDetector.StartDetection(OnSpeedHackDetected);

		// we may react on saves alteration
		ObscuredPrefs.onAlterationDetected = SavesAlterationDetected;
#if !UNITY_FLASH

		// and even may react on foreign saves (from another device)
		ObscuredPrefs.onPossibleForeignSavesDetected = ForeignSavesDetected;
#endif

		detectorsUsageExample = (DetectorsUsageExample)FindObjectOfType(typeof(DetectorsUsageExample));
	}

	private void SavesAlterationDetected()
	{
		savesAlterationDetected = true;
	}

#if !UNITY_FLASH
	private void ForeignSavesDetected()
	{
		foreignSavesDetected = true;
	}
#endif

	private void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();

		CenteredLabel("Memory cheating protection");

		GUILayout.Space(10);

		if (obscuredStringTest && obscuredStringTest.enabled)
		{
			if (GUILayout.Button("Use regular string"))
			{
				obscuredStringTest.UseRegular();
			}

			if (GUILayout.Button("Use obscured string"))
			{
				obscuredStringTest.UseObscured();
			}

			string currentString;

			if (obscuredStringTest.useRegular)
			{
				currentString = obscuredStringTest.cleanString;
			}
			else
			{
				currentString = obscuredStringTest.obscuredString;
			}
			GUILayout.Label("Current string (try to change it!):\n" + currentString);
		}

		if (obscuredIntTest && obscuredIntTest.enabled)
		{
			GUILayout.Space(10);
			if (GUILayout.Button("Use regular int (click to generate new number)"))
			{
				obscuredIntTest.UseRegular();
			}

			if (GUILayout.Button("Use ObscuredInt (click to generate new number)"))
			{
				obscuredIntTest.UseObscured();
			}

			int currentLivesCount;

			if (obscuredIntTest.useRegular)
			{
				currentLivesCount = obscuredIntTest.cleanLivesCount;
			}
			else
			{
				currentLivesCount = obscuredIntTest.obscuredLivesCount;
			}
			GUILayout.Label("Current lives count (try to change them!):\n" + currentLivesCount);
		}

#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
		GUILayout.BeginHorizontal();
		GUILayout.Label("ObscuredInt from inspector: " + dummyObscuredInt);
		if (GUILayout.Button("+"))
		{
			dummyObscuredInt++;
		}
		if (GUILayout.Button("-"))
		{
			dummyObscuredInt--;
		}
		GUILayout.EndHorizontal();
#endif

		if (obscuredFloatTest && obscuredFloatTest.enabled)
		{
			GUILayout.Space(10);
			if (GUILayout.Button("Use regular float (click to generate new number)"))
			{
				obscuredFloatTest.UseRegular();
			}

			if (GUILayout.Button("Use ObscuredFloat (click to generate new number)"))
			{
				obscuredFloatTest.UseObscured();
			}

			float currentHealthBar;

			if (obscuredFloatTest.useRegular)
			{
				currentHealthBar = obscuredFloatTest.healthBar;
			}
			else
			{
				currentHealthBar = obscuredFloatTest.obscuredHealthBar;
			}
			GUILayout.Label("Current health bar (try to change it!):\n" + System.String.Format("{0:0.000}", currentHealthBar));
		}

		if (obscuredVector3Test && obscuredVector3Test.enabled)
		{
			GUILayout.Space(10);
			if (GUILayout.Button("Use regular Vector3 (click to generate new one)"))
			{
				obscuredVector3Test.UseRegular();
			}

			if (GUILayout.Button("Use ObscuredVector3 (click to generate new one)"))
			{
				obscuredVector3Test.UseObscured();
			}

			Vector3 position;

			if (obscuredVector3Test.useRegular)
			{
				position = obscuredVector3Test.playerPosition;
			}
			else
			{
				position = obscuredVector3Test.obscuredPlayerPosition;
			}
			GUILayout.Label("Current player position (try to change it!):\n" + position);
		}

		GUILayout.Space(10);

		GUILayout.EndVertical();
		GUILayout.Space(10);
		GUILayout.BeginVertical();
		CenteredLabel("Saves cheating protection");
		GUILayout.Space(10);

		if (obscuredPrefsTest && obscuredPrefsTest.enabled)
		{
			if (GUILayout.Button("Save game with regular PlayerPrefs!"))
			{
				obscuredPrefsTest.SaveGame(false);
			}

			if (GUILayout.Button("Read data saved with regular PlayerPrefs"))
			{
				obscuredPrefsTest.ReadSavedGame(false);
			}

			GUILayout.Space(10);

			if (GUILayout.Button("Save game with ObscuredPrefs!"))
			{
				obscuredPrefsTest.SaveGame(true);
			}

			if (GUILayout.Button("Read data saved with ObscuredPrefs"))
			{
				obscuredPrefsTest.ReadSavedGame(true);
			}

			ObscuredPrefs.preservePlayerPrefs = GUILayout.Toggle(ObscuredPrefs.preservePlayerPrefs, "preservePlayerPrefs");

#if UNITY_EDITOR
			ObscuredPrefs.unobscuredMode = GUILayout.Toggle(ObscuredPrefs.unobscuredMode, "unobscuredMode");
#endif

#if !UNITY_FLASH
			ObscuredPrefs.emergencyMode = GUILayout.Toggle(ObscuredPrefs.emergencyMode, "emergencyMode");

			GUILayout.Label("LockToDevice level:");
			savesLock = GUILayout.SelectionGrid(savesLock, new[] { ObscuredPrefs.DeviceLockLevel.None.ToString(), ObscuredPrefs.DeviceLockLevel.Soft.ToString(), ObscuredPrefs.DeviceLockLevel.Strict.ToString() }, 3);
			ObscuredPrefs.lockToDevice = (ObscuredPrefs.DeviceLockLevel)savesLock;

			ObscuredPrefs.readForeignSaves = GUILayout.Toggle(ObscuredPrefs.readForeignSaves, "readForeignSaves");
#endif

			GUILayout.Label("PlayerPrefs: \n" + obscuredPrefsTest.gameData);

			if (savesAlterationDetected)
			{
				GUILayout.Label("Saves were altered! }:>");
			}
#if !UNITY_FLASH
			if (foreignSavesDetected)
			{
				GUILayout.Label("Saves more likely from another device! }:>");
			}
#endif
		}

		if (detectorsUsageExample != null)
		{
			GUILayout.Label("Speed hack detected: " + detectorsUsageExample.speedHackDetected);
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID
			GUILayout.Label("Injection detected: " + detectorsUsageExample.injectionDetected);
#else
			GUILayout.Label("Injection detection is not available on current platform");
#endif
			GUILayout.Label("Obscured type cheating detected: " + detectorsUsageExample.obscuredTypeCheatDetected);
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void CenteredLabel(string caption)
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(caption);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}