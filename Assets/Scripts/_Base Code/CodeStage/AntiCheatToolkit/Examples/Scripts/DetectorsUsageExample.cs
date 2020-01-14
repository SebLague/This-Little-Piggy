using CodeStage.AntiCheat.Detectors;
using UnityEngine;

// Simple detectors usage examples.
public class DetectorsUsageExample : MonoBehaviour
{
	// just to allow ActTesterGUI draw labels with detectoin statuses
	[HideInInspector]
	public bool injectionDetected = false;

	[HideInInspector]
	public bool speedHackDetected = false;

	[HideInInspector]
	public bool obscuredTypeCheatDetected = false;

	void Start ()
	{
		// SpeedHackDetector usage example.
		// In this case we subscribe to the speed hack detection event,
		// set detector update interval to 1 second, allowing 5 false positives and
		// allowing cooldown after 60 seconds (read more about cooldown in the readme.pdf).
		// Thus OnSpeedHackDetected normally will execute after 5 seconds since 
		// speed hack was applied to the application.
		// Please, note, we have SpeedHackDetector added to the test scene, but all settings
		// we made there in inspecor will be overriden by settings we pass
		// to the SpeedHackDetector.StartDetection(); e.g.:
		// SpeedHackDetector.StartDetection(OnSpeedHackDetected, 1f, 5, 60);

		// for now, we'll just start detection and keep using settings set in inspector:
		SpeedHackDetector.StartDetection(OnSpeedHackDetected);

// InjectionDetector supports only these platforms
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID
		// InjectionDetector usage example.
		// We may change some options like this ...
		InjectionDetector.Instance.autoDispose = true;
		InjectionDetector.Instance.keepAlive = true;

		// ... and subscribe to the injection detection like this
		InjectionDetector.StartDetection(OnInjectionDetected);
#endif

		// similar subsciption to the Obscured types cheating
		// works for all Obscured types except ObscuredPrefs (it has own cheating detection algos)
		ObscuredCheatingDetector.StartDetection(OnObscuredTypeCheatingDetected);
	}

	private void OnSpeedHackDetected()
	{
		speedHackDetected = true;
		Debug.LogWarning("Speed hack detected!");
	}

	private void OnInjectionDetected()
	{
		injectionDetected = true;
		Debug.LogWarning("Injection detected!");
	}

	private void OnObscuredTypeCheatingDetected()
	{
		obscuredTypeCheatDetected = true;
		Debug.LogWarning("Obscured type cheating detected!");
	}
}
