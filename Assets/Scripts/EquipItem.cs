using UnityEngine;
using System.Collections;

public abstract class EquipItem : MyBase {

	[HideInInspector]
	public bool isRanged;
	[HideInInspector]
	public EquippedItemHandling handling;
	[HideInInspector]
	public bool useItemHandling;

	public bool allowAds;
	[HideInInspector]
	public bool isPlayerWeapon;
	[HideInInspector]
	public int ammoTotalCounter = 100;
	[HideInInspector]
	public int ammoMagCounter = 100;
	public GameObject pickup;

	[HideInInspector]
	public Animator animator;

	[HideInInspector]
	public bool isGun;


	public override void Awake ()
	{
		base.Awake ();
		if (GetComponent<Animator>())
			animator = GetComponent<Animator>();
		if (GetComponent<EquippedItemHandling>()) {
			useItemHandling = true;
			handling = GetComponent<EquippedItemHandling>();
		}
	}

	public abstract void LeftMouseDown();

	public abstract void LeftMouseUp();

	public abstract void LeftMouseHold();

	public void SetAds(bool isAds) {
		if (useItemHandling && allowAds) {
			handling.SetADS(isAds);
		}
	}

	public void SetRunning(bool running) {
		if (useItemHandling) {
			handling.SetRunning(running);
		}
	}


	public void Bob(float walkSpeed,float runSpeed, float deltaDistance) {
		if (useItemHandling) {
			handling.Bob(walkSpeed,runSpeed,deltaDistance);
		}
	}

	public void Sway(float deltaX, float deltaY) {
		if (useItemHandling) {
			handling.Sway(deltaX,deltaY);
		}
	}

	public virtual void Die(Vector3 gravCentre) {
		if (!isPlayerWeapon && pickup != null) {
	
			Transform p = Instantiate(pickup.transform,transform.position, Quaternion.identity) as Transform;
			Vector3 gravityUp = (p.position - gravCentre).normalized;
			Vector3 localUp = p.up;
			
			Quaternion targetRotation = Quaternion.FromToRotation(localUp,gravityUp) * p.rotation;
			p.rotation = targetRotation;

			Destroy(gameObject);
		}
	}

	public virtual void Reload() {
	}

	public abstract void ChangeGravity(FauxGravityAttractor attractor);
}
