using UnityEngine;
using System.Collections;

public class GravityObject : MyBase {

	public enum DefaultGravity  {Subordinate,Friendly,Hostile};
	public DefaultGravity defaultGravity;

	[HideInInspector]
	public FauxGravityBody myGravity;

	private bool hasJustPortalled;

	[HideInInspector]
	public bool inFriendlyWorld;

	public override void Awake ()
	{
		base.Awake ();
		myGravity = GetComponent<FauxGravityBody>();

		if (defaultGravity == DefaultGravity.Friendly) {
			SetGravity( GameObject.FindGameObjectWithTag("Friendly Planet").GetComponent<FauxGravityAttractor>());
		}
		else if (defaultGravity == DefaultGravity.Hostile) {
			SetGravity(GameObject.FindGameObjectWithTag("Hostile Planet").GetComponent<FauxGravityAttractor>());
		}

	
	}

	public virtual void OnTriggerEnter(Collider c) {
		if (c.tag == "Portal" && !hasJustPortalled) {
			Teleport();
			hasJustPortalled = true;
			myGravity.attractor = c.gameObject.GetComponent<Portal>().Teleport(myTransform);
			UpdateGravity();
			Invoke("ResetPortalTime",.5f);
		}
	}

	void ResetPortalTime() {
		hasJustPortalled = false;
	}

	public virtual void OnTriggerExit(Collider c) {
		//hasJustPortalled = false;
	}
	
	public virtual void UpdateGravity() {
		inFriendlyWorld = myGravity.attractor.friendlyWorld;
	}

	public virtual void Teleport() {

	}
	


	public void SetGravity(FauxGravityAttractor attractor) {
		if (attractor) {
			myGravity.attractor = attractor;
		}
		UpdateGravity();
	}
	
}
