using UnityEngine;
using System.Collections;

public class EquippedItemHandling : MonoBehaviour {

	// Key positions
	public Vector3 hipfirePos;
	public Vector3 hipfireRot;
	public Vector3 adsPos;
	public Vector3 runPos;
	public Vector3 runRot;
	public Vector3 reloadPos;
	public Vector3 reloadRot;


	float changeHoldSpeed = 4;
	Vector3 targetHoldPos;
	Vector3 targetHoldRot;

	// Recoil
	public Vector3 deltaRecoil;
	public Vector3 randomDeltaRecoilMin;
	public Vector3 randomDeltaRecoilMax;
	float recoilReturnToRestSpeed = 5;
	Vector3 recoilValue;

	// Gun Sway
	public float adsSwayDamper = .2f;
	float currentSwayDamper = 1;

	float rotVelocityY;
	float rotVelocityX;
	float targetRotY;
	float targetRotX;
	float currentRotY;
	float currentRotX;

	// Gun Bob
	public float adsBobDamper = .2f;
	
	public float idleBobSpeed;
	public float walkBobSpeed;
	public float runBobSpeed;
	public float idleBobSize;
	public float walkBobSize;
	public float runBobSize;

	float currentBobDamper = 1;
	float targetBobSize;
	float currentBobSpeed;
	float currentBobSize;
	float bobSizeV;
	float bobValue;


	// States
	bool isADS;
	bool isRunning;
	bool isChangingHoldPos;
	bool isChangingHoldRot;
	[HideInInspector]
	public bool reloading;


	void Awake() {
		transform.localEulerAngles = Vector3.zero;
		transform.localPosition = hipfirePos;
		targetHoldPos = hipfirePos;
		SetTargetHoldRot(hipfireRot);
	}


	void Update () {
		recoilValue = Vector3.MoveTowards(recoilValue,Vector3.zero,Time.deltaTime * recoilReturnToRestSpeed);

		// Changing hold pos, e.g. idle, running, ads
		if (isChangingHoldPos) {
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,targetHoldPos,changeHoldSpeed * Time.deltaTime);
			if (transform.localPosition == targetHoldPos) {
				isChangingHoldPos = false;
				currentBobSize = 0;
			}
		}
		else {
			// Gun bob
			bobValue += Time.deltaTime * currentBobSpeed;
			currentBobSize = Mathf.SmoothDamp(currentBobSize,targetBobSize,ref bobSizeV,.5f);
			float x = Mathf.Sin(bobValue) * currentBobSize;
			float y = Mathf.Cos(bobValue*2) * -currentBobSize;
			transform.localPosition = targetHoldPos + recoilValue + new Vector3(x,y);
		}

		// Changing hold rotation, e.g. idle, running, ads
		if (isChangingHoldRot) {
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation,Quaternion.Euler(targetHoldRot),5 * 180 * Time.deltaTime);
			if (transform.localEulerAngles == targetHoldRot) {
				isChangingHoldRot = false;
			}
		}
		else {
			if (!reloading) {
				// Gun sway
				currentRotY = Mathf.SmoothDampAngle(currentRotY,targetRotY,ref rotVelocityY,.3f);
				currentRotX = Mathf.SmoothDampAngle(currentRotX,targetRotX,ref rotVelocityX,.3f);
				transform.localEulerAngles = new Vector3(currentRotX,currentRotY);
			}
		}


	}

	// Sway the gun as character turns
	public void Sway(float xRot, float yRot) {
			targetRotY = yRot * currentSwayDamper;
			targetRotX = xRot * currentSwayDamper;

	}

	// Bob the gun as character moves
	public void Bob(float walkSpeed, float runSpeed, float currentSpeed) {
		if (currentSpeed == 0 && !reloading) { /// Idle 
			targetBobSize = idleBobSize;
			currentBobSpeed = idleBobSpeed;
		}
		else if (isRunning && !isADS && !reloading) { /// Running 
			targetBobSize = 1;
			currentBobSpeed = Mathf.Lerp(walkBobSpeed,runBobSpeed,currentSpeed/runSpeed);
			currentBobSize = walkBobSize;
		}
		else { /// Walking 
			currentBobSpeed = Mathf.Lerp(idleBobSpeed,walkBobSpeed,currentSpeed/walkSpeed);
			targetBobSize = walkBobSize;
		}

		currentBobSize *= currentBobDamper;


	}

	// Set the gun's ads state
	public void SetADS(bool ads) {
		isADS = ads;
		if (!reloading) {
			if (isADS) {
				currentSwayDamper = adsSwayDamper;
				currentBobDamper = adsBobDamper; 
				SetTargetHoldPos(adsPos);
				SetTargetHoldRot(Vector3.zero);
			}
			else {
				currentSwayDamper = 1;
				currentBobDamper = 1;
				if (isRunning) {
					SetRunning(true);
				}
				else {
					SetTargetHoldPos(hipfirePos);
					SetTargetHoldRot(hipfireRot);
				}
			}
		}

	}

	public void Reload(float reloadTime) {
		if (!reloading) {
			reloading = true;
			StartCoroutine("AnimateReload",reloadTime);

		}
	}

	IEnumerator AnimateReload(float t) {
		SetTargetHoldPos(reloadPos);
		SetTargetHoldRot(reloadRot);
		//print (t);
		yield return new WaitForSeconds(t);
		reloading = false;
		if (isRunning) {
			SetTargetHoldPos(runPos);
			SetTargetHoldRot(runRot);
		}
		else if (isADS) {
			SetTargetHoldPos(adsPos);
			SetTargetHoldRot(Vector3.zero);
		}
		else {
			SetTargetHoldPos(hipfirePos);
			SetTargetHoldRot(hipfireRot);
		}

	}

	// Set the character's sprint state
	public void SetRunning (bool running) {
		isRunning = running;
		if (!reloading) {
			if (!isADS) {
				if (isRunning) {
					SetTargetHoldPos(runPos);
					SetTargetHoldRot(runRot);
				}
				else {
					SetTargetHoldPos(hipfirePos);
					SetTargetHoldRot(hipfireRot);
				}
			}
		}
	}

	public void TransitionFromAnim() {
		isChangingHoldPos = true;
		isChangingHoldRot = true;
	}

	public  bool QueryCanShoot() {
		return !isRunning && !reloading;
	}

	public void Recoil() {
		recoilValue = deltaRecoil + new Vector3(Random.Range(randomDeltaRecoilMin.x,randomDeltaRecoilMax.x),Random.Range(randomDeltaRecoilMin.y,randomDeltaRecoilMax.y),Random.Range(randomDeltaRecoilMin.z,randomDeltaRecoilMax.z));
	}

	private void SetTargetHoldPos(Vector3 newHoldPos) {
		isChangingHoldPos = true;
		targetHoldPos = newHoldPos;
	}

	private void SetTargetHoldRot(Vector3 newHoldRot) {
		isChangingHoldRot = true;
		targetHoldRot = newHoldRot;
	}

	public bool IsAimingDownSights {
		get {
			return isADS;
		}
	}
}
