using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	Vector3 originalPos;
	bool shaking;

	float duration;
	float intesity;
	float magnitude;
	float verticalMagnitude;
	float rotForce;

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			Shake(.8f, 8, 10, 3, 20);
		}
	}

	public void Shake(float shakeDuration, float shakeIntensity, float shakeMagnitude, float shakeVerticalMagnitude, float shakeRotForce) {

		duration = shakeDuration;
		intesity = shakeIntensity;
		magnitude = shakeMagnitude;
		verticalMagnitude = shakeVerticalMagnitude;
		rotForce = shakeRotForce;

		if (shaking) {
			StopCoroutine("AnimateShake");
			transform.localPosition = originalPos;
		}
		StartCoroutine("AnimateShake");
	}
	

	IEnumerator AnimateShake() {
		shaking = true;

		// System:
		originalPos = transform.localPosition;

		float percent = 0;
		float percentIncreaseSpeed = 1f/duration;

		float maxSwitchTime = 1f/intesity;
		float minSwitchTime = .05f;
		float timeToNextSwitch = 0;

		float directionX = 1;
		float currentForce = 0;
		float smoothV = 0;

		float nextSignSwitch = 0;

		float rotSmoothV = 0;
		float rot = 0;

		while (percent < 1) {
			percent += percentIncreaseSpeed * Time.deltaTime;
			float damper = Mathf.Clamp01(1-percent);

			// Change shake direction
			if (percent > nextSignSwitch) {
				float currentSwitchTime = Mathf.Lerp(maxSwitchTime,minSwitchTime,percent);
				timeToNextSwitch = currentSwitchTime/percentIncreaseSpeed;
				nextSignSwitch = percent + currentSwitchTime;

				directionX *= -1;
			}

			float targetForce = magnitude * directionX;
			currentForce = Mathf.SmoothDamp(currentForce,targetForce,ref smoothV, timeToNextSwitch);

			// Calculate vertical force
			float forceY = Mathf.Sin(Time.time * intesity)/2 * verticalMagnitude;

			transform.localPosition = originalPos + new Vector3(currentForce,forceY,0) * damper;

			// Rotation
			if (rotForce != 0) {
				float targetRot = rotForce * -directionX;
				rot = Mathf.SmoothDampAngle(rot,targetRot,ref rotSmoothV,timeToNextSwitch);
				transform.localEulerAngles = new Vector3(0,0,rot) * damper;
			}
			yield return null;

		}
		shaking = false;

	}


}
