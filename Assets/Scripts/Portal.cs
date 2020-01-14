using UnityEngine;
using System.Collections;

public class Portal : MyBase {
	public bool toFriendly;
	public FauxGravityAttractor newWorldGravity;
	public Transform pairedPortal;
	private Transform player;
	public Transform worldCam;
	public Transform centrePoint;
	private Vector3 centre;

	public LayerMask mask;
	public Transform playerLook;

	public static bool activateCams;




	public override void Awake() {
		base.Awake();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		centre = centrePoint.position;
	}





	void Update() {
		if (activateCams) {
			if (worldCam) {
				//print("TR");

				Ray ray = new Ray(worldCam.position,playerLook.forward);
			//	Debug.DrawRay(ray.origin,ray.direction * 100,Color.red,5);
				RaycastHit hit;
				if (Physics.Raycast(ray,out hit,Mathf.Infinity,mask)) {
				//	print ("HIT");
					worldCam.LookAt(hit.point);
				}


			}
		}
	}

	public FauxGravityAttractor Teleport(Transform t) {
		//Vector3 deltaP = centre - t.position;
		t.position = worldCam.position;
		if (t.tag == "Player") {
			t.GetComponent<Rigidbody>().AddForce(t.forward * 300);
		}
		return newWorldGravity;
	}

}
