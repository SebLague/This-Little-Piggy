using UnityEngine;
using System.Collections;

public class DynamicCameraMovement : MonoBehaviour {
	
	public bool isAnimated;
	
	// Edit these vars to modify camera constant rotation
	public float rotSpeed = .2f;
	public float rotDegrees = 0.5f;

	// Edit these vars to modify camera look at mouse
	public float lookRotSpeed = 2f;
	public float lookRotDegrees = 1f;
	
	
	private Vector3 initialGameCamRot;
	private float rotTime = 0;
	private float currentLookX = 0;
	private float currentLookY = 0;
	private bool disabled;
	
	void Awake() {
		initialGameCamRot = transform.eulerAngles;	
	}
	

	void LateUpdate() {
		if (disabled)
			return;
		
		if (isAnimated)
			initialGameCamRot = transform.eulerAngles;
		
		// Mouse
		Vector3 mouse = Input.mousePosition;
		float mouseX = (mouse.x - Screen.width/2) / (Screen.width/2);
		float mouseY = (mouse.y - Screen.height/2) / (Screen.height/2);
		mouseX = Mathf.Clamp(mouseX,-1f,1f);
		mouseY = Mathf.Clamp(mouseY,-1f,1f);
		currentLookX = Mathf.Lerp(currentLookX, mouseX * lookRotDegrees, Time.deltaTime * lookRotSpeed);
		currentLookY = Mathf.Lerp(currentLookY, mouseY * lookRotDegrees, Time.deltaTime * lookRotSpeed);
		Vector3 lookRot = new Vector3(- currentLookY,currentLookX);
		
		
		// Constant rotation
		float x = Mathf.Sin(rotTime );
		float y = Mathf.Sin(2*rotTime);
		rotTime+= Time.deltaTime*rotSpeed;
		transform.eulerAngles = initialGameCamRot + new Vector3(x,y) * rotDegrees + lookRot;
	}
	
	public void DisableDynamicMovement() {
		//disabled = true;	
	}
}
