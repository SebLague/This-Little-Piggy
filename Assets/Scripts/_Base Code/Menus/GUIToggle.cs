using UnityEngine;
using System.Collections;

public class GUIToggle : GUIInput {

	public delegate void OnToggle(string parent, string inputName, bool state);
	public static OnToggle onToggle;

	private bool toggleState;

	public override void Awake () {
		base.Awake ();

		bool.TryParse(PlayerPrefs.GetString(id), out toggleState);
	}

	void OnMouseDown() {
		toggleState = !toggleState;
		PlayerPrefs.SetString(id, toggleState.ToString());

		onToggle(parentMenuName,inputName,toggleState);
	}
}
