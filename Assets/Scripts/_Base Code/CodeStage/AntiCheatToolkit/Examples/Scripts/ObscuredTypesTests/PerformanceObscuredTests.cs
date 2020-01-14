using System.Diagnostics;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// These super simple and stupid tests allow you to see how slower Obscured types can be compared to the regular types.
/// Take in account iterations count though.
/// </summary>
public class PerformanceObscuredTests : MonoBehaviour
{
#if !UNITY_FLASH
	public bool boolTest = true;
	public int boolIterations = 2500000;

	public bool byteTest = true;
	public int byteIterations = 2500000;

	public bool shortTest = true;
	public int shortIterations = 2500000;

	public bool ushortTest = true;
	public int ushortIterations = 2500000;

	public bool intTest = true;
	public int intIterations = 2500000;

	public bool uintTest = true;
	public int uintIterations = 2500000;

	public bool longTest = true;
	public int longIterations = 2500000;

	public bool floatTest = true;
	public int floatIterations = 2500000;

	public bool doubleTest = true;
	public int doubleIterations = 2500000;

	public bool stringTest = true;
	public int stringIterations = 250000;

	public bool vector3Test = true;
	public int vector3Iterations = 2500000;

	public bool prefsTest = true;
	public int prefsIterations = 2500;

	private void Start()
	{
		Invoke("StartTests", 1f);
	}

	private void StartTests()
	{
		Debug.Log("--- OBSCURED TYPES PERFORMANCE TESTS ---\n");

		if (boolTest) TestBool();
		if (byteTest) TestByte();
		if (shortTest) TestShort();
		if (ushortTest) TestUShort();
		if (intTest) TestInt();
		if (uintTest) TestUInt();
		if (longTest) TestLong();
		if (floatTest) TestFloat();
		if (doubleTest) TestDouble();
		if (stringTest) TestString();
		if (vector3Test) TestVector3();
		if (prefsTest) TestPrefs();
	}

	private void TestBool()
	{
		Debug.Log("  Testing ObscuredBool vs bool preformance:\n  " + boolIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();

		ObscuredBool obscured = true;
		bool notObscured = obscured;
		bool dummy = false;

		for (int i = 0; i < boolIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < boolIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredBool:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < boolIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < boolIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    bool:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy){}
		if (obscured) { }
		if (notObscured) { }
	}

	private void TestByte()
	{
		Debug.Log("  Testing ObscuredByte vs byte preformance:\n  " + byteIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();

		ObscuredByte obscured = 100;
		byte notObscured = obscured;
		byte dummy = 0;

		for (int i = 0; i < byteIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < byteIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredByte:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < byteIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < byteIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    byte:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != 0) { }
		if (obscured != 0) { }
		if (notObscured != 0) { }
	}

	private void TestShort()
	{
		Debug.Log("  Testing ObscuredShort vs short preformance:\n  " + shortIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();

		ObscuredShort obscured = 100;
		short notObscured = obscured;
		short dummy = 0;

		for (int i = 0; i < shortIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < shortIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredShort:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < shortIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < shortIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    short:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != 0) { }
		if (obscured != 0) { }
		if (notObscured != 0) { }
	}

	private void TestUShort()
	{
		Debug.Log("  Testing ObscuredUShort vs ushort preformance:\n  " + ushortIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();

		ObscuredUShort obscured = 100;
		ushort notObscured = obscured;
		ushort dummy = 0;

		for (int i = 0; i < ushortIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < ushortIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredUShort:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < ushortIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < ushortIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    ushort:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != 0) { }
		if (obscured != 0) { }
		if (notObscured != 0) { }
	}

	private void TestDouble()
	{
#if !UNITY_FLASH
		Debug.Log("  Testing ObscuredDouble vs double preformance:\n  " + doubleIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();

		ObscuredDouble obscured = 100d;
		double notObscured = obscured;
		double dummy = 0;

		for (int i = 0; i < doubleIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < doubleIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredDouble:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < doubleIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < doubleIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    double:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != 0) { }
		if (obscured != 0) { }
		if (notObscured != 0) { }
#endif
	}

	private void TestFloat()
	{
		Debug.Log("  Testing ObscuredFloat vs float preformance:\n  " + floatIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();

		ObscuredFloat obscured = 100f;
		float notObscured = obscured;
		float dummy = 0;

		for (int i = 0; i < floatIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < floatIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredFloat:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < floatIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < floatIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    float:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != 0) { }
		if (obscured != 0) { }
		if (notObscured != 0) { }
	}

	private void TestInt()
	{
		Debug.Log("  Testing ObscuredInt vs int preformance:\n  " + intIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();
		ObscuredInt obscured = 100;
		int notObscured = obscured;
		int dummy = 0;

		for (int i = 0; i < intIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < intIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredInt:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < intIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < intIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    int:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != 0) { }
		if (obscured != 0) { }
		if (notObscured != 0) { }
	}

	private void TestLong()
	{
#if !UNITY_FLASH
		Debug.Log("  Testing ObscuredLong vs long preformance:\n  " + longIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();
		ObscuredLong obscured = 100L;
		long notObscured = obscured;
		long dummy = 0;

		for (int i = 0; i < longIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < longIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredLong:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < longIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < longIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    long:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != 0) { }
		if (obscured != 0) { }
		if (notObscured != 0) { }
#endif
	}

	private void TestString()
	{
		Debug.Log("  Testing ObscuredString vs string preformance:\n  " + stringIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();
		ObscuredString obscured = "abcd";
		string notObscured = obscured;
		string dummy = "";

		for (int i = 0; i < stringIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < stringIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredString:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < stringIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < stringIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    string:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != "") { }
		if (obscured != "") { }
		if (notObscured != "") { }
	}

	private void TestUInt()
	{
		Debug.Log("  Testing ObscuredUInt vs uint preformance:\n  " + uintIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();
		ObscuredUInt obscured = 100u;
		uint notObscured = obscured;
		uint dummy = 0;

		for (int i = 0; i < uintIterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < uintIterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredUInt:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < uintIterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < uintIterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    uint:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != 0) { }
		if (obscured != 0) { }
		if (notObscured != 0) { }
	}

	private void TestVector3()
	{
		Debug.Log("  Testing ObscuredVector3 vs Vector3 preformance:\n  " + vector3Iterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();
		ObscuredVector3 obscured = new Vector3(1f, 2f, 3f);
		Vector3 notObscured = obscured;
		Vector3 dummy = new Vector3(0, 0, 0);

		for (int i = 0; i < vector3Iterations; i++)
		{
			dummy = obscured;
		}

		for (int i = 0; i < vector3Iterations; i++)
		{
			obscured = dummy;
		}

		Debug.Log("    ObscuredVector3:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < vector3Iterations; i++)
		{
			dummy = notObscured;
		}

		for (int i = 0; i < vector3Iterations; i++)
		{
			notObscured = dummy;
		}

		sw.Stop();
		Debug.Log("    Vector3:\n    " + sw.ElapsedMilliseconds + "  ms ");

		if (dummy != Vector3.zero) { }
		if (obscured != Vector3.zero) { }
		if (notObscured != Vector3.zero) { }
	}

	private void TestPrefs()
	{
		Debug.Log("  Testing ObscuredPrefs vs PlayerPrefs preformance:\n  " + prefsIterations + " iterations for read and same for write");

		Stopwatch sw = Stopwatch.StartNew();

		for (int i = 0; i < prefsIterations; i++)
		{
			ObscuredPrefs.SetInt("__a",1);
			ObscuredPrefs.SetFloat("__b",2f);
			ObscuredPrefs.SetString("__c","3");
		}

		for (int i = 0; i < prefsIterations; i++)
		{
			ObscuredPrefs.GetInt("__a", 1);
			ObscuredPrefs.GetFloat("__b", 2f);
			ObscuredPrefs.GetString("__c", "3");
		}

		Debug.Log("    ObscuredPrefs:\n    " + sw.ElapsedMilliseconds + " ms ");
		sw.Reset();

		sw.Start();
		for (int i = 0; i < prefsIterations; i++)
		{
			PlayerPrefs.SetInt("__a", 1);
			PlayerPrefs.SetFloat("__b", 2f);
			PlayerPrefs.SetString("__c", "3");
		}

		for (int i = 0; i < prefsIterations; i++)
		{
			PlayerPrefs.GetInt("__a", 1);
			PlayerPrefs.GetFloat("__b", 2f);
			PlayerPrefs.GetString("__c", "3");
		}

		sw.Stop();
		Debug.Log("    PlayerPrefs:\n    " + sw.ElapsedMilliseconds + "  ms ");

		PlayerPrefs.DeleteKey("__a");
		PlayerPrefs.DeleteKey("__b");
		PlayerPrefs.DeleteKey("__c");
	}
#endif
}