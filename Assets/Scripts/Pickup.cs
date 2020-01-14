using UnityEngine;
using System.Collections;

public class Pickup : MyBase {
	public enum Action {Equip, Pickup, AutoPickup};
	public Action action;
	public bool rotate;

	public EquipItem equipItem;
	public string name;
	public bool customDisplay;
	public string customDisplayText;
	public bool disabled;
	public bool autoDestruct;

	public AudioClip[] Sounds;

	float displayTime = 2;

	void Awake() {
		base.Awake();
		if (rotate) {
			StartCoroutine(Rotate());
		}

		if (autoDestruct) {
			Destroy(gameObject,15);
		}
	}

	IEnumerator Rotate() {
		while (true) {
			yield return null;
			transform.Rotate(Vector3.up * 130 * Time.deltaTime);
		}
	}

	public void Used() {
		if (Sounds.Length >0) {
			PlayAudio(Sounds[0]);
		}
		transform.position = Vector3.up * 1000;
		Destroy(gameObject,.5f);
	}

	public void OnTriggerEnter(Collider c) {
		if (!disabled) {
			if (c.tag == "Player") {
				if (customDisplay) {
					HUD.ShowMessage(customDisplayText, displayTime);
				}
				else
					HUD.ShowMessage("Press f to take " + name.ToLower(), displayTime);
			}
		}
	}

	public void OnTriggerExit(Collider c) {

	}
}
