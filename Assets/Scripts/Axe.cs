using UnityEngine;
using System.Collections;

public class Axe : EquipItem {

	public AudioClip slashAudio;
	public AudioClip impactAudio;
	public LayerMask impactLayer;


	public float damage = 5;
	public Transform head;


	public override void Awake ()
	{
		base.Awake ();
		animator.enabled = false;
	}



	public void Impact() {
		Collider[] cs = Physics.OverlapSphere(head.position,.4f,impactLayer);
		if(cs.Length > 0) {
			PlayAudio(impactAudio);
		}
		foreach (Collider c in cs) {
			if (!(c.tag == "Player" && isPlayerWeapon)) {
				IDamageable hitObject = c.transform.GetComponent(typeof(IDamageable)) as IDamageable;
				if (hitObject != null) {
					hitObject.TakeDamage(damage,true);
				}
			}
		}
		Invoke("DisableAnim",.2f);
	}

	void PlaySwing() {
		PlayAudio(slashAudio);
	}


	public override void LeftMouseHold ()
	{
	}
	

	public override void LeftMouseDown ()
	{
		//StartCoroutine("Attack");
		if (animator.enabled != true) {
			Invoke("PlaySwing",.2f);
			animator.enabled = true;
			animator.SetTrigger("Hack");
		}

	}

	void DisableAnim() {
		animator.enabled = false;
		handling.TransitionFromAnim();
	}

	public override void LeftMouseUp ()
	{

	}

	public override void ChangeGravity (FauxGravityAttractor attractor)
	{

	}
}
