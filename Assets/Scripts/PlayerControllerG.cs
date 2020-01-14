using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class PlayerControllerG : MonoBehaviour {
	/*
	public Gun gun;
	private GunHandling gunHandling;

	// Handling
	public float walkSpeed = 4;
	public float runSpeed = 7f;
	public float jumpHeight = 5;
	public float gravity = 8;
	public float sensitivity = 6;

	// Components
	private CharacterController controller;
	private Transform camT;

	// System
	private float verticalVelocity;
	private float rotOldY;
	private float rotOldX;

	void Start() {
		gunHandling = gun.gameObject.GetComponent<GunHandling>();
		controller = GetComponent<CharacterController>();
		camT = Camera.main.transform;
		Screen.lockCursor = true;
		rotOldY = transform.eulerAngles.y;
		rotOldX = camT.eulerAngles.x;
	}

	void Update() {
		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
		verticalVelocity -= gravity * Time.deltaTime;

		bool running = (Input.GetButton("Run") && !gunHandling.IsAimingDownSights);
		float speedMod = ((running)?runSpeed:walkSpeed);
		Vector3 moveAmount = input.normalized * speedMod + Vector3.up * verticalVelocity;

		controller.Move(transform.TransformDirection(moveAmount) * Time.deltaTime);

		// Look rotation
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime);
		camT.Rotate (Vector3.left * Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime);
		camT.localEulerAngles = Vector3.right * LimitRot(50,285,camT.localEulerAngles.x);

		// Grounded
		if (controller.collisionFlags == CollisionFlags.Below) {
			verticalVelocity = 0;

			if (Input.GetButtonDown("Jump")) {
				verticalVelocity = jumpHeight;
			}
		}

		// Gun Handling

		// Sway
		float rotNewY = transform.eulerAngles.y;
		float deltaY = GetDeltaRot(rotNewY,rotOldY);
		rotOldY = rotNewY;

		float rotNewX = camT.eulerAngles.x;
		float deltaX = GetDeltaRot(rotNewX,rotOldX);
		rotOldX = rotNewX;

		gunHandling.Sway(deltaX,deltaY);

		// Aim down sights
		if (Input.GetMouseButtonDown(1)) {
			gunHandling.SetADS(true);
		}

		else if (Input.GetMouseButtonUp(1)) {
			gunHandling.SetADS(false);
		}

		// Set movement
		if (Input.GetButtonDown("Run")) {
			gunHandling.SetRunning(true);
		}	
		if (Input.GetButtonUp("Run")) {
			gunHandling.SetRunning(false);
		}

		// Gun bob
		float deltaDistance = input.magnitude * speedMod;
		gunHandling.Bob(walkSpeed,runSpeed,deltaDistance);

		// Shoot
		if (Input.GetMouseButtonDown(0)) {
			gun.PullTrigger();
		}
		else if (Input.GetMouseButtonUp(0)) {
			gun.ReleaseTrigger();
		}
		else if (Input.GetMouseButton(0)) {
			gun.HoldDownTrigger();
		}


	}

	float GetDeltaRot(float r1, float r2) {
		r1%=360;
		r2%=360;
		float greater = Mathf.Max(r1,r2);
		float lesser = Mathf.Min(r1,r2);
		float deltaRot = greater-lesser;
		if (deltaRot > 180) {
			deltaRot = (lesser-greater) %360;
		}
		float dir = Mathf.Sign(r1-r2);
		return deltaRot * dir;
	}



	// Clamps look rot in an fps.
	private float LimitRot(float lessThan, float greaterThan, float currentRot) {
		float rot = currentRot - lessThan;
		rot = (rot + 360) % 360;
		if (rot < greaterThan - lessThan) {
			return (rot > (greaterThan - lessThan)/2)?greaterThan:lessThan;
		}
		return currentRot;
	}

*/
}
