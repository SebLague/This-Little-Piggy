using UnityEngine;
using System.Collections;

public class MainMenu : Menu {

	public Transform mainMenu;
	public Transform optionsMenu;
	public Transform creditsMenu;

	public override void Awake ()
	{
		base.Awake ();
		AudioManager.Init();
	}

	public override void OnButtonDown (string input)
	{
		switch(input) {
		case "play":
			Application.LoadLevel("GameLevel");
			break;
		case "options":
			mainMenu.gameObject.SetActive(false);
			optionsMenu.gameObject.SetActive(true);
			break;
		case "credits":
			mainMenu.gameObject.SetActive(false);
			creditsMenu.gameObject.SetActive(true);
			break;
		case "back":
			mainMenu.gameObject.SetActive(true);
			optionsMenu.gameObject.SetActive(false);
			creditsMenu.gameObject.SetActive(false);
			break;
		default:

			break;
		}

	}

	public override void OnSlider (string input, float percent, float value)
	{
		switch(input) {
		case "master volume":
			AudioManager.VolumeMaster = percent;
			break;
		case "music volume":
			AudioManager.VolumeMusic = percent;
			break;
		case "sfx volume":
			AudioManager.VolumeSfx = percent;
			break;
		}
	}
}
