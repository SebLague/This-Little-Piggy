using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ObscuredVector3Test : MonoBehaviour
{
	// This is some valuable value for cheater.
	// Sometimes they wish to teleport player for example.
	// This variable can be easily found in memory by different special
	// tools (like CheatEngine, ArtMoney, etc.) since it is stored in memory
	// as is, clean and clear.
	internal Vector3 playerPosition = new Vector3(10.5f, 11.5f, 12.5f);

	// But if you'll store your player position as ObscuredVector3, cheater will 
	// not be able to find and modify it as easily as with native Vector3!
	// Cheater will need to dive deep into your code to figure out how
	// position is stored to figure out how to reach it in memory.
	// All this raises bar of cheater's skills and knowledge and increases 
	// amount of time he need to spend on your game thus this will help reduce 
	// your game cheaters count dramatically!
	internal ObscuredVector3 obscuredPlayerPosition = new Vector3(10.5f, 11.5f, 12.5f);

	internal bool useRegular = true;

	private void Start()
	{
		Debug.Log("===== ObscuredVector3Test =====\n");

		// example of custom crypto key using
		// this is not necessary! key is 120205 by default
		ObscuredVector3.SetNewCryptoKey(404);

		// just a small self-test here
		playerPosition = new Vector3(54.1f,64.3f,63.2f);
		Debug.Log("Original position:\n" + playerPosition);

		obscuredPlayerPosition = playerPosition;
		//Vector3 encrypted = obscuredPlayerPosition.GetEncrypted();
		ObscuredVector3.RawEncryptedVector3 encrypted = obscuredPlayerPosition.GetEncrypted();
		Debug.Log("How your position is stored in memory when obscured:\n(" + encrypted.x + ", " + encrypted.y + ", " + encrypted.z + ")");
	}

	public void UseRegular()
	{
		useRegular = true;
		playerPosition += new Vector3(Random.Range(-10f, 50f), Random.Range(-10f, 50f), Random.Range(-10f, 50f));
		obscuredPlayerPosition = new Vector3(10.5f, 11.5f, 12.5f);
		Debug.Log("Try to change this Vector3 in memory:\n" + playerPosition);
	}

	public void UseObscured()
	{
		useRegular = false;
		obscuredPlayerPosition += new Vector3(Random.Range(-10f, 50f), Random.Range(-10f, 50f), Random.Range(-10f, 50f));
		playerPosition = new Vector3(10.5f, 11.5f, 12.5f); ;
		Debug.Log("Try to change this Vector3 in memory:\n" + obscuredPlayerPosition);
	}
}
