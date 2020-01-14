using UnityEngine;
using System.Collections;

public interface IDamageable {

	void TakeDamage(float dmg,bool isPlayer);
	void Die();

}