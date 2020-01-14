using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public class Lifeform : GravityObject, IDamageable {

	public ObscuredFloat health;
	[HideInInspector]
	public bool dead;

	[HideInInspector]
	public bool isPlayer;

	[HideInInspector]
	public Animator anim;

	[HideInInspector]
	public float vulnerability = 1;

	public override void Awake ()
	{
		base.Awake ();
		if (GetComponent<Animator>())
			anim = GetComponent<Animator>();
	}


	public virtual void TakeDamage(float dmg, bool playerDamage) {
		dmg *= vulnerability;
		if (!isPlayer && playerDamage) {

			Player.AddScore(Mathf.Clamp(dmg,0,health));
		}
		if (isPlayer != playerDamage) {
			health -= dmg;
		}

	

		if (health <= 0) 
			Die();
	}

	public virtual void Die() {
		dead = true;

	}


}
