using UnityEngine;
using System.Collections;

public class IdolDestroy : MonoBehaviour {

	Vector3 force;
	int frame;


	void Awake() {

		GetComponent<FauxGravityBody>().attractor = GameObject.FindGameObjectWithTag("Hostile Planet").GetComponent<FauxGravityAttractor>();
		force = (transform.up + Random.insideUnitSphere*.2f * 100);
		Destroy(gameObject,20);
	}

	void FixedUpdate() {
		frame ++;
		if (frame <20) {
			GetComponent<Rigidbody>().AddForce(force);
		}
	}
}
