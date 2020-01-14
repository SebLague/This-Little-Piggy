using UnityEngine;
using System.Collections;

public class Character : Lifeform {

	public Transform hands;
	[HideInInspector]
	public EquipItem equippedItem;

	public virtual void EquipItem(EquipItem item) {
		if (item) {
			if (equippedItem != null) {
				Destroy(equippedItem.gameObject);
			}
			
			equippedItem = Instantiate(item,hands.position,hands.rotation) as EquipItem;
			equippedItem.transform.parent = hands;
			
			UpdateGravity();
		}
	}
}
