using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ObscuredPrefsTest : MonoBehaviour
{
	// This is a small trick - it allows to hide your encryption key 
	// in the serialized MonoBehaviour in the Editor inpector, 
	// outside of the IL bytecode, so to recover it hacker needs to 
	// know how to reach it in the Unity scene ;)
	public string encryptionKey = "change me!";

	internal string gameData = "";

	private void OnApplicationQuit()
	{
		PlayerPrefs.DeleteKey("money");
		PlayerPrefs.DeleteKey("lifeBar");
		PlayerPrefs.DeleteKey("playerName");

		ObscuredPrefs.DeleteKey("money");
		ObscuredPrefs.DeleteKey("lifeBar");
		ObscuredPrefs.DeleteKey("playerName");
		ObscuredPrefs.DeleteKey("gameComplete");
		ObscuredPrefs.DeleteKey("demoLong");
		ObscuredPrefs.DeleteKey("demoDouble");
		ObscuredPrefs.DeleteKey("demoByteArray");
		ObscuredPrefs.DeleteKey("demoVector3");
	}

	private void Awake()
	{
		ObscuredPrefs.SetNewCryptoKey(encryptionKey);
	}

	public void SaveGame(bool obscured)
	{
		if (obscured)
		{
			ObscuredPrefs.SetInt("money", 1500);
			ObscuredPrefs.SetFloat("lifeBar", 25.9f);
			ObscuredPrefs.SetString("playerName", "focus xD");
			ObscuredPrefs.SetBool("gameComplete", true);
#if !UNITY_FLASH
			ObscuredPrefs.SetLong("demoLong", 3457657543456775432L);
			ObscuredPrefs.SetDouble("demoDouble", 345765.1312315678d);
#endif
			ObscuredPrefs.SetByteArray("demoByteArray", new byte[]{44, 104, 43, 32});
			ObscuredPrefs.SetVector3("demoVector3", new Vector3(123.312f, 453.12345f,1223f));
			
			Debug.Log("Game saved using ObscuredPrefs. Try to find and change saved data now! ;)");
		}
		else
		{
			PlayerPrefs.SetInt("money", 2100);
			PlayerPrefs.SetFloat("lifeBar", 88.4f);
			PlayerPrefs.SetString("playerName", "focus :D");
			Debug.Log("Game saved with regular PlayerPrefs. Try to find and change saved data now (it's easy)!");
		}
		ObscuredPrefs.Save();
	}

	public void ReadSavedGame(bool obscured)
	{
		if (obscured)
		{
			
			gameData = "Money: " + ObscuredPrefs.GetInt("money") + "\n";
			gameData += "Life bar: " + ObscuredPrefs.GetFloat("lifeBar") + "\n";
			gameData += "Player name: " + ObscuredPrefs.GetString("playerName") + "\n";
			gameData += "bool: " + ObscuredPrefs.GetBool("gameComplete") + "\n";
#if !UNITY_FLASH
			gameData += "long: " + ObscuredPrefs.GetLong("demoLong") + "\n";
			gameData += "double: " + ObscuredPrefs.GetDouble("demoDouble") + "\n";
#endif
			byte[] ba = ObscuredPrefs.GetByteArray("demoByteArray", 0, 4);
			gameData += "Vector3: " + ObscuredPrefs.GetVector3("demoVector3") + "\n";
			gameData += "byte[]: {" + ba[0] + "," + ba[1] + "," + ba[2] + "," + ba[3] + "}";
		}
		else
		{
			gameData = "Money: " + PlayerPrefs.GetInt("money") + "\n";
			gameData += "Life bar: " + PlayerPrefs.GetFloat("lifeBar") + "\n";
			gameData += "Player name: " + PlayerPrefs.GetString("playerName");
		}
	}
}