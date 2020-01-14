using UnityEngine;
using System.Collections;

public class Idol : MyBase, IDamageable {

	public float health;
	bool dead;
	public GameObject destroyed;
	public AudioClip Explosion;

	public void TakeDamage(float dmg, bool isPlayer) {
		if (isPlayer) {
			health -= dmg;
		}
		if (health <= 0) {
			Die ();
		}
	}

	public void Die ()
	{
		if (!dead) {
			dead= true;

			PlayAudio(Explosion);
			Instantiate(destroyed,transform.position,transform.rotation);
			EnemySpawnManager.IdolDestroyed();
			Destroy(gameObject);
		}
	}

}
