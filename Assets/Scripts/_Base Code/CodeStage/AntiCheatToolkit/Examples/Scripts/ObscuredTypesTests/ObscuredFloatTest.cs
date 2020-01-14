using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ObscuredFloatTest: MonoBehaviour
{
	// This is some valuable value for cheater.
	// Sometimes they wish to have freezed health bar for example.
	// This variable can be easily found in memory by different special
	// tools (like CheatEngine, ArtMoney, etc.) since it is stored in memory
	// as is, clean and clear.
	internal float healthBar = 11.4f;

	// But if you'll store your health bar as ObscuredFloat, cheater will 
	// not be able to find and modify it as easily as with native float!
	// Cheater will need to dive deep into your code to figure out how
	// health bar is stored to figure out how to reach it in memory.
	// All this raises bar of cheater's skills and knowledge and increases 
	// amount of time he need to spend on your game thus this will help reduce 
	// your game cheaters count dramatically!
	internal ObscuredFloat obscuredHealthBar = 11.4f;

	internal bool useRegular = true;

	private void Start()
	{
		Debug.Log("===== ObscuredFloatTest =====\n");

		// example of custom crypto key using
		// this is not necessary! key is 230887 by default
		ObscuredFloat.SetNewCryptoKey(404);

		// just a small self-test here
		healthBar = 99.9f;
		Debug.Log("Original health bar:\n" + healthBar);

		obscuredHealthBar = healthBar;
		Debug.Log("How your health bar is stored in memory when obscured:\n" + obscuredHealthBar.GetEncrypted());

		float totalProgress = 100f;
		ObscuredFloat currentProgress = 60.3f;
		ObscuredFloat.SetNewCryptoKey(666); // you can change crypto key at any time!

		// all usual operations supported, dummy code, just for demo purposes
		currentProgress ++;
		currentProgress -= 2;
		currentProgress --;
		currentProgress = totalProgress - currentProgress;

		obscuredHealthBar = healthBar = currentProgress = 0;
	}

	public void UseRegular()
	{
		useRegular = true;
		healthBar += Random.Range(-10f, 50f);
		obscuredHealthBar = 11f;
		Debug.Log("Try to change this float in memory:\n" + healthBar);
	}

	public void UseObscured()
	{
		useRegular = false;
		obscuredHealthBar += Random.Range(-10f, 50f);
		healthBar = 11f;
		Debug.Log("Try to change this float in memory:\n" + obscuredHealthBar);
	}
}