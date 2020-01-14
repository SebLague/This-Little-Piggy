using UnityEngine;
using System.Collections;

public class FauxGravityAttractor : MonoBehaviour {

	public float gravity = -12;
	public bool friendlyWorld;

	public void Attract(Transform body, float customGravity, bool dontAffectForce,bool dontAffectRot) {
		Vector3 gravityUp = (body.position - transform.position).normalized;
		Vector3 localUp = body.up;
		if (!dontAffectForce)
			body.GetComponent<Rigidbody>().AddForce(gravityUp * customGravity);

		if (!dontAffectRot) {
			Quaternion targetRotation = Quaternion.FromToRotation(localUp,gravityUp) * body.rotation;
		
			body.rotation = Quaternion.Slerp(body.rotation,targetRotation,Mathf.Abs(customGravity) * 4 * Time.deltaTime );
		}
	}   

	public void Attract(Transform body, float customGravity, bool dontAffectForce) {
		Attract(body,customGravity,dontAffectForce,false);
	}

	public void Attract(Transform body, float customGravity) {
		Attract(body,customGravity,true);
	}


	public void Attract(Transform body, bool dontAffectForce) {
		Attract(body,gravity,dontAffectForce);
	}

	public void Attract(Transform body) {
		Attract(body,gravity,true);
	}

}
