using UnityEngine;
using System.Collections;

/*
 * Provides a deltaTime independent from timescale.
 * Only use where continued motion during pause is desired.
 */

public class RealTime : MonoBehaviour {
	
	public static float deltaTime;
	private float realTimeOld;

	void Awake() {
		realTimeOld = Time.realtimeSinceStartup;
	}

	void Update() {
		RealTime.deltaTime = Time.realtimeSinceStartup - realTimeOld;
		realTimeOld = Time.realtimeSinceStartup;
	}

	void OnApplicationPause(bool pauseState) {
		realTimeOld = Time.realtimeSinceStartup;
	}
}
