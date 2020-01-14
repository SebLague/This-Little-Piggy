using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ObscuredIntTest: MonoBehaviour
{
	// This is some valuable value for cheater.
	// Sometimes they wish to have unlimited lives for example.
	// This variable can be easily found in memory by different special
	// tools (like CheatEngine, ArtMoney, etc.) since it is stored in memory
	// as is, clean and clear.
	internal int cleanLivesCount = 11;

	// But if you'll store your lives count as ObscuredInt, cheater will 
	// not be able to find and modify them as easily as with native int!
	// Cheater will need to dive deep into your code to figure out how
	// lives are stored and encrypted to try search for the encrypted value.
	// All this raises bar of cheater's skills and knowledge and increases 
	// amount of time he need to spend on your game thus this will help reduce 
	// your game cheaters count dramatically!
	internal ObscuredInt obscuredLivesCount = 11;

	internal bool useRegular;

	private void Start()
	{
		Debug.Log("===== ObscuredIntTest =====\n");

		// example of custom crypto key using
		// this is not necessary! key is 444444 by default
		//ObscuredInt.SetNewCryptoKey(434523131);

		// just a small self-test here
		cleanLivesCount = 5;
		Debug.Log("Original lives count:\n" + cleanLivesCount);

		obscuredLivesCount = cleanLivesCount;
		Debug.Log("How your lives count is stored in memory when obscured:\n" + obscuredLivesCount.GetEncrypted());

		ObscuredInt.SetNewCryptoKey(666); // you can change crypto key at any time!

		// unprotected
		const int livesCount = 100;

		// protected!
		ObscuredInt livesCountSecure = 100;

		// all usual operations supported
		livesCountSecure -= 10; 
		livesCountSecure = livesCountSecure + livesCount;
		livesCountSecure = livesCountSecure / 10;

		ObscuredInt.SetNewCryptoKey(888); // you can change crypto key at any time!

		livesCountSecure++;

		ObscuredInt.SetNewCryptoKey(999); // you can change crypto key at any time!

		livesCountSecure++;
		livesCountSecure--;

		// will print "Lives count: 20 (14h)"
		Debug.Log("Lives count: " + livesCountSecure + " (" + livesCountSecure.ToString("X") + "h)");
	}

	public void UseRegular()
	{
		useRegular = true;
		cleanLivesCount += Random.Range(-10, 50);
		obscuredLivesCount = 11;
		Debug.Log("Try to change this int in memory:\n" + cleanLivesCount);
	}

	public void UseObscured()
	{
		useRegular = false;
		obscuredLivesCount += Random.Range(-10, 50);
		cleanLivesCount = 11;
		Debug.Log("Try to change this int in memory:\n" + obscuredLivesCount);
	}
}