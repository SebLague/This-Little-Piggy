using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class PlaceItem : MonoBehaviour {


	void Update () {
		transform.LookAt(Vector3.zero);
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + -90,transform.localEulerAngles.y,transform.localEulerAngles.z);

		Ray ray = new Ray(transform.position,-transform.up);
		RaycastHit hit;
		if (Physics.Raycast(ray,out hit)) {
			transform.position = hit.point;
		}
	}
}
