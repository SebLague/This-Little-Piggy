using UnityEngine;
using System.Collections;

public class FauxRagdoll : MyBase {

	public GameObject[] affectors;

	public float force;
	public int duration = 100;
	private int frame;
	private bool stopped;

	private Vector3 centreOfGrav;

	public override void Awake () {
		base.Awake();

		foreach (GameObject g in affectors) {

			g.GetComponent<Rigidbody>().useGravity = false;

		}



	}

	void FixedUpdate() {
		if (frame < duration) {
			frame ++;
			foreach (GameObject g in affectors) {
				g.GetComponent<Rigidbody>().AddForce((transform.position - centreOfGrav).normalized* -force);
			}
		}
		else if (!stopped) {
			stopped = true;
			foreach (GameObject g in affectors) {
				if (g.GetComponent<CharacterJoint>()) {
					Destroy(g.GetComponent<CharacterJoint>());
				}
				Destroy(g.GetComponent<Rigidbody>());
			}
		}
	}

	public void SetGrav(Vector3 c) {
		centreOfGrav = c;
	}

}
