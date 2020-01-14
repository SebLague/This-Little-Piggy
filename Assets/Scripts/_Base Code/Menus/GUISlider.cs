using UnityEngine;
using System.Collections;

public class GUISlider : GUIInput {

	public bool retainInput;
	public Transform knob;
	public float minValue;
	public float maxValue;
	public float defaultPercent;

	public delegate void SliderMoved(string parent, string inputName, float percent, float value);
	public static SliderMoved sliderMoved;

	bool mouseDown;
	float currentPercent;
	Vector3 centre;
	float halfWidth;
	float minX;
	float maxX;
	float knobXOld;

	public override void Awake () {
		base.Awake ();

		centre = myTransform.position;
		halfWidth = boundSize.x/2;
		minX = centre.x - halfWidth;
		maxX = centre.x + halfWidth;

		if (retainInput)
			defaultPercent = PlayerPrefs.GetFloat(id, defaultPercent);

		currentPercent = defaultPercent;
		SetKnobPosition();
		TriggerSlider();
	}

	void Update() {
		if (mouseDown) {
			float x = GetMousePosition().x;
			currentPercent = Mathf.InverseLerp(minX,maxX,x);
			SetKnobPosition();

			if (knob.position.x != knobXOld && sliderMoved != null)
				TriggerSlider();
		}
	}

	void TriggerSlider() {
		if (sliderMoved != null)
			sliderMoved(parentMenuName,inputName,currentPercent,Mathf.Lerp(minValue,maxValue,currentPercent));
	}

	void OnMouseDown() {
		mouseDown = true;
	}

	void OnMouseUp() {
		mouseDown = false;
	}

	void SetKnobPosition() {
		knobXOld = knob.position.x;
		knob.position = centre + Vector3.right * Mathf.Lerp(-halfWidth,halfWidth,currentPercent);
	}


	void OnDisable() {
		if (retainInput)
			PlayerPrefs.SetFloat(id, currentPercent);
	}



}
