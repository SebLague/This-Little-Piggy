using UnityEngine;
using System.Collections;

public class PickupBacon : MyBase {
	public AudioClip mySound;
	
	public void Used(Player p) {
		if(mySound)
			PlayAudio(mySound);
		p.health += 10;
		p.UpdateHud ();
		Destroy(gameObject);
	}

	public void OnTriggerEnter(Collider c) {
			if (c.tag == "Player") {
				Used (c.GetComponent<Player>());
			}
	}

}
