using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ObscuredStringTest: MonoBehaviour
{
	// This variable can be easily found in memory by different special
	// tools (like CheatEngine, ArtMoney, etc.) since it is stored in memory
	// as is, clean and clear.
	internal string cleanString;

	// But if you'll store your string as ObscuredString, cheater will 
	// not be able to find and modify it as easily as with native string!
	// Cheater will need to dive deep into your code to figure out how
	// string is stored and encrypted to try search for the encrypted string.
	// All this raises bar of cheater's skills and knowledge and increases amount
	// of time he need to spend on your game thus this will help reduce 
	// your game cheaters count dramatically!
	internal ObscuredString obscuredString;

	internal bool useRegular;

	private void Start()
	{
		Debug.Log("===== ObscuredStringTest =====\n");

		// example of custom crypto key using
		// this is not necessary! default key is "4441"
		ObscuredString.SetNewCryptoKey("I LOVE MY GIRL");

		// just a small self-test here (hey, Daniele! :D)
		cleanString = "Try Goscurry! Or better buy it!";
		Debug.Log("Original string:\n" + cleanString);

		obscuredString = cleanString;
		Debug.Log("How your string is stored in memory when obscured:\n" + obscuredString.GetEncrypted());

		obscuredString = cleanString = "";
	}

	public void UseRegular()
	{
		useRegular = true;
		cleanString = "Hey, you can easily change me in memory!";
		obscuredString = "";
		Debug.Log("Try to change this string in memory:\n" + cleanString);
	}

	public void UseObscured()
	{
		useRegular = false;
		obscuredString = "Hey, you can't change me in memory!";
		cleanString = "";
		Debug.Log("Try to change this string in memory:\n" + obscuredString);
	}
}