using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class GUIInput : MyBase {

	public string inputName;
	public string parentMenuName; // The menu which this input item belongs to

	[HideInInspector]
	public string id; // Unique id for saving data in playerprefs

	[HideInInspector]
	public Vector2 boundSize;

	[HideInInspector]
	public Camera guiCam;

	public override void Awake ()
	{
		base.Awake ();
		BoxCollider2D bounds = GetComponent<BoxCollider2D>();
		boundSize = new Vector2(bounds.size.x * myTransform.localScale.x, bounds.size.y * myTransform.localScale.y);
		guiCam = GameObject.FindGameObjectWithTag("GUI Camera").GetComponent<Camera>();

		id = Application.loadedLevelName + "." + parentMenuName + "." + inputName;
	}

	protected Vector2 GetMousePosition() {
		Vector3 mousePos = guiCam.ScreenToWorldPoint(Input.mousePosition);
		return new Vector2(mousePos.x,mousePos.y);
	}

}
