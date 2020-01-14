using UnityEngine;
using System.Collections;

public class GUIButton : GUIInput {

	public delegate void OnButton(string parent, string inputName);
	public static OnButton onButton;

	// Colour fading
	public Color selectedColour;
	private Color defaultColour;
	private Material mat;

	private const float colourFadeSpeed = 5;
	private float fadePercent;
	private TextMesh textMesh;
	private bool useTextMesh;

	public AudioClip selectAudio;

	public override void Awake () {
		base.Awake ();
		if (GetComponent<TextMesh>()) {
			useTextMesh = true;
			textMesh = GetComponent<TextMesh>();
			defaultColour = textMesh.color;
		}
		else  {
			mat = GetComponent<Renderer>().material;
			defaultColour = mat.color;
		}
	}

	// Button pressed
	void OnMouseDown() {
		//print ("!");
		if (onButton != null)
			onButton(parentMenuName, inputName);
	}

	// Mouse over button
	void OnMouseEnter() {
		StopCoroutine("AnimateColour");
		StartCoroutine("AnimateColour", true);
	}

	// Mouse exited button
	void OnMouseExit() {
		StopCoroutine("AnimateColour");
		StartCoroutine("AnimateColour", false);
	}


	// Fade button colour
	IEnumerator AnimateColour(bool selected) {
		int target = ((selected)?1:0);
		int dir = ((selected)?1:-1);

		while (fadePercent != target) {
			yield return null;
			fadePercent = Mathf.Clamp01(fadePercent + RealTime.deltaTime * colourFadeSpeed * dir);
			if (useTextMesh)
				textMesh.color = Color.Lerp(defaultColour,selectedColour, fadePercent);
			else
				mat.color = Color.Lerp(defaultColour,selectedColour, fadePercent);
		}
	}
}
