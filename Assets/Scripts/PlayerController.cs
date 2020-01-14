using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MyBase {

	// Handling
	public float walkSpeed = 4;
	public float runSpeed = 7f;
	public float jumpForce = 320;
	public float gravity = 8;
	public float sensitivity = 6;
	public Transform feetPosition;
	public LayerMask groundMask;

	// Components
	[HideInInspector]
	public Vector3 gravCentre;
	private Transform camT;
	private Player player;

	// System
	private int airborneDir;
	private Vector3 moveAmount;
	private bool grounded;

	private Vector3 airborneV;

	
	private float rotOldY;
	private float rotOldX;
	bool jumping;

	public AudioClip[] Footsteps;
	float timeLastStep;

	
	void Start() {
		timeLastStep = Time.time;
		player = GetComponent<Player>();
		camT = Camera.main.transform;
		Screen.lockCursor = true;
		StartCoroutine("AirborneDirection");

		airborneDir = -1;
	}

	void Update() {
		Screen.lockCursor = true;
		Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));

		float speedMod = ((Input.GetButton("Run"))?runSpeed:walkSpeed);
		moveAmount = input.normalized * speedMod;
		if (!grounded && !jumping) {
			airborneDir = -1;

		}
	
		if (grounded && (Input.GetAxisRaw("Horizontal")!=0 || Input.GetAxisRaw("Vertical") != 0)) {
			if(timeLastStep + 1f-(0.1f*speedMod) < Time.time) {
				PlayAudio(Footsteps[Random.Range(0, Footsteps.Length)]);
				timeLastStep=Time.time;
			}
		}
		
		// Look rotation
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime);
		camT.Rotate (Vector3.left * Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime);
		camT.localEulerAngles = Vector3.right * LimitRot(50,285,camT.localEulerAngles.x);
		grounded = false;

		// Check if touching ground (only if moving downwards)
		if (airborneDir == -1) {
			Ray ray = new Ray(feetPosition.position, -transform.up);
			RaycastHit hit;
			/// Debug.DrawRay(ray.origin,ray.direction * 1,Color.red,1);
			if (Physics.Raycast(ray,out hit,1,groundMask)) {
				grounded = true;
			}
		}

		if (grounded) {
			if (airborneV != Vector3.zero) {
			//	print ("FAULT");
				jumping = false;
				airborneV = Vector3.zero;
		}
			if (Input.GetButtonDown("Jump")) {
				jumping=true;
				GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
				airborneDir = 1;
			//	airborneV = input.normalized * speedMod/1.5f;
				//print(moveAmount + "");

			}
		}

		// Item Handling
		#region Handling
		
		
		// Sway
		float rotNewY = transform.eulerAngles.y;
		float deltaY = GetDeltaRot(rotNewY,rotOldY);
		rotOldY = rotNewY;
		
		float rotNewX = camT.eulerAngles.x;
		float deltaX = GetDeltaRot(rotNewX,rotOldX);
		rotOldX = rotNewX;

		
		if (player.equippedItem) {
			EquipItem item = player.equippedItem;
			item.Sway(deltaX,deltaY);
			
			// Aim down sights
			if (Input.GetMouseButtonDown(1)) {
				item.SetAds(true);
			}
			
			else if (Input.GetMouseButtonUp(1)) {
				item.SetAds(false);
			}
			
			// Set movement
			if (Input.GetButtonDown("Run")) {
				item.SetRunning(true);
			}	
			if (Input.GetButtonUp("Run")) {
				item.SetRunning(false);
			}
			
			// Gun bob
			float deltaDistance = input.magnitude * speedMod;
			item.Bob(walkSpeed,runSpeed,deltaDistance);
		}
		#endregion

	}

	void OnEnable() {
		StartCoroutine("AirborneDirection");
	}

	void OnDisable() {
		StopCoroutine("AirborneDirection");
	}

	// Get vertical direction in the air (up = 1; down = -1)
	IEnumerator AirborneDirection() {
		float dstOld = 0;
		Vector3 posOld = Vector3.zero;
		bool hasPosOld = false;
		bool newJump = true;
		while (true) {
			yield return new WaitForSeconds(.1f);
			if (!grounded && airborneDir == 1) {
				if (newJump) {
					yield return new WaitForSeconds(.3f);
					newJump = false;
				}
				if (hasPosOld) {
					Vector3 posNew = transform.position;
					//print (Vector3.Distance(gravCentre,posNew) + "   " + Vector3.Distance(gravCentre,posOld));
					if (Vector3.Distance(gravCentre,posNew) < Vector3.Distance(gravCentre,posOld)) {
						airborneDir = -1;
						newJump = true;
					//	print ("Down");
					}

					posOld = posNew;
				}
				else {
					hasPosOld = true;
					posOld = transform.position;
				}
			}
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


	void FixedUpdate() {
		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveAmount) * Time.deltaTime);
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


}
