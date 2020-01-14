using UnityEngine;
using System.Collections;

public class IntroCamera : MonoBehaviour {

	public GameObject childCam;
	private CameraShake shake;
	


	public void ActivatePortalIntro() {
		shake = childCam.GetComponent<CameraShake>();
		//shake.Shake(2,5,2,.5f,.3f);
		shake.Shake(1.5f,18,1.2f,.5f,.3f);
	}


	void Update () {
	
	}
}
