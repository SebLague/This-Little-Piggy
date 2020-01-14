using UnityEngine;
using System.Collections;

public class Shell : GravityObject {

	private const float fadeSpeed = .1f;
	public Renderer r;
	private float lifeTime = 5;
	private Color startColour;
	private float startFadeTime;
	private float fadePercent;
	private Material mat;

	void Start () {
		mat = r.material;
		startColour = mat.color;
		startFadeTime = Time.time + lifeTime;
		StartCoroutine("Fade");
	}

	IEnumerator Fade() {
		while (true) {
			yield return new WaitForSeconds(.2f);
			if (Time.time > startFadeTime) {
				fadePercent = Mathf.MoveTowards(fadePercent,1,fadeSpeed);
				mat.color = Color.Lerp(startColour,Color.clear,fadePercent);
				if (fadePercent >= 1)
					GameObject.Destroy(this.gameObject);
			}
		}
	}
	
}
