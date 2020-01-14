using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class FauxGravityBody : MonoBehaviour {

	public bool dontAffectForce;
	public bool useCustomGravity;
	public float customGravity;
	public FauxGravityAttractor attractor;
	private Transform myTransform;
	public bool dontAffectRotation;

	void Start () {
		//attractor = GameObject.FindGameObjectWithTag("Planet").GetComponent<FauxGravityAttractor>();
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

		myTransform = transform;
	}

	void FixedUpdate () {
		if (attractor){
			if (dontAffectRotation) {
				attractor.Attract(myTransform,customGravity,dontAffectForce,dontAffectRotation);
			}
			else {
				if (useCustomGravity) 
					attractor.Attract(myTransform,customGravity,dontAffectForce);
				else
					attractor.Attract(myTransform,dontAffectForce);
			}
		}
	}
	
}
