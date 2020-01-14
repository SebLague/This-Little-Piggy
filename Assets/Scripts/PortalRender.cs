using UnityEngine;
using System.Collections;

public class PortalRender : MonoBehaviour {

	public GameObject cam;
	bool visible = true;

	bool inBounds = false;



	void OnTriggerEnter(Collider c) {
		if (c.tag == "Player") {
			inBounds = true;
			cam.SetActive(true);
		}
	}

	void OnTriggerExit(Collider c) {
		if (c.tag == "Player") {
			inBounds = false;
			cam.SetActive(false);
		}
	}
	
}
