using UnityEngine;
using System.Collections;

public class GameOver : Menu {

	public override void Awake() {
		base.Awake();
		Screen.lockCursor = false;
	}

	public override void OnButtonDown (string input)
	{

		switch(input) {
		case "back":
			//print("!");
			//print (input);
			Application.LoadLevel("Menu");
			break;
		default:
			
			break;
		}
		
	}
	
	public override void OnSlider (string input, float percent, float value)
	{
	}
}
