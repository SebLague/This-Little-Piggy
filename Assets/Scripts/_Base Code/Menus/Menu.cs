using UnityEngine;
using System.Collections;

public abstract class Menu : MonoBehaviour {

	public string menuName; // this must be the same as GUIInput's parentMenuName to be affected by that input object.

	public virtual void Awake() {
		GUIButton.onButton += ManageButtonDown;
		GUISlider.sliderMoved += ManageSlider;
	}


	// A button belonging to this menu has been pressed
	public abstract void OnButtonDown(string input);

	// A slider belonging to this menu has been modified
	public abstract void OnSlider(string input, float percent, float value);

	// On Destroy
	public virtual void OnDestroy() {
		GUIButton.onButton -= ManageButtonDown;
		GUISlider.sliderMoved -= ManageSlider;
	}

	// System
	#region System

	private void ManageButtonDown(string parent, string input) {
		if (parent == menuName)
			OnButtonDown(input);
	}
	
	private void ManageSlider(string parent, string input, float percent, float value) {
		if (parent == menuName)
			OnSlider(input,percent,value);
	}

	#endregion
}
