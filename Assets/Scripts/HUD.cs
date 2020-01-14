using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	
	public TextMesh scoreText;
	public TextMesh ammoText;
	public TextMesh healthText;
	public TextMesh dialogueText;
	
	public GameObject objectives;
	public TextMesh[] objectiveText;
	private string[] objectiveTitles;
	public Color objectiveCompletedColour;
	
	string[] currentTextSequence;
	float[] currentTimeSequence;
	
	string[] introText;
	float[] introTimes;
	
	string[] makeSaladText;
	float[] makeSaladTimes;
	
	string[] fetchAxeText;
	float[] fetchAxeTimes;
	
	string[] pigKillText;
	float[] pigKillTimes;
	
	string[] portalText;
	float[] portalTimes;
	
	string[] finishThemText;
	float[] finishThemTime;
	
	string[] victoryText;
	float[] victoryTimes;
	
	
	bool showingTextSequence;
	
	public static HUD instance;
	int sequenceIndex;
	
	void Awake() {
		instance = this;
		objectives.SetActive(false);
		introText = new string[3];
		introTimes = new float[introText.Length];
		
		introText[0] = "Hello ...";
		introText[1] = "Welcome to your new life as a space hermit.";
		introText[2] = "Today we'll be making delicious potato salad!";
		
		introTimes[0] = 2f;
		introTimes[1] = 4.5f;
		introTimes[2] = 4.5f;
		
		// Make salad
		makeSaladText = new string[1];
		makeSaladTimes = new float[introText.Length];
		
		makeSaladText[0] = "Nice! Get back to your shack and prepare the salad\non the plate outside.";
		makeSaladTimes[0] = 4f;
		
		// Fetch axe
		fetchAxeText = new string[2];
		fetchAxeTimes = new float[introText.Length];
		
		fetchAxeText[0] = "Well, frankly it's a bit bland.\nCould do with some bacon!";
		fetchAxeText[1] = "There should be an axe lodged\nin a tree stump somewhere...";
		
		fetchAxeTimes[0] = 3.5f;
		fetchAxeTimes[1] = 4f;
		
		// Kill pig
		pigKillText = new string[2];
		pigKillTimes = new float[introText.Length];
		
		pigKillText[0] = "Hurrah!";
		pigKillText[1] = "Wait... what's that noise?";
		
		pigKillTimes[0] = 2f;
		pigKillTimes[1] = 2.5f;
		
		// Portal
		portalText = new string[3];
		portalTimes = new float[introText.Length];
		
		portalText[0] = "Oh balls";
		portalText[1] = "The pig people have built a portal to your dimension.";
		portalText[2] = "You'll have to destroy the pig idols in their world\nto stop them from reincarnating.";
		
		portalTimes[0] = 2f;
		portalTimes[1] = 3.5f;
		portalTimes[2] = 4.5f;
		
		// Portal
		finishThemText = new string[1];
		finishThemTime = new float[introText.Length];
		
		finishThemText[0] = "That's all the idols gone,\nnow finish off the last of the pig people.";
		
		finishThemTime[0] = 2f;
		
		
		// Victory
		victoryText = new string[2];
		victoryTimes = new float[introText.Length];
		
		victoryText[0] = "Hey woah, you did it!";
		victoryText[1] = "All right then soldier, go get your potato salad.\nYou've earned it!";
		
		victoryTimes[0] = 3f;
		victoryTimes[1] = 4.5f;
		
		
		ForceShowMessage(introText,introTimes,0);
		
	}
	
	public void SetNewObjective(params string[] names) {
		objectives.SetActive(true);
		objectiveTitles = new string[names.Length];
		for (int i = objectiveText.Length-1; i >=0; i --) {
			objectiveText[i].color = Color.white;
			if (i < names.Length) {
				objectiveText[i].gameObject.SetActive(true);
				objectiveText[i].text = names[i];
				objectiveTitles[i] = names[i];
				
			}
			else {
				objectiveText[i].gameObject.SetActive(false);
			}
		}
		
	}
	
	public void UpdateObjective(params int[] completion) {
		int j = 0;
		for (int i = 0; i < completion.Length/2; i++) {
			string c = "";
			//if (completion[j+1] >1) {
			c = " " + completion[j]+ "/" +completion[j+1];
			
			//if (objectiveTitles[i] != null)
			objectiveText[i].text = objectiveTitles[i] + c;
			
			if (completion[j] >= completion[j+1]) {
				objectiveText[i].color = objectiveCompletedColour;
			}
			
			j+=2;
		}
	}
	
	public void SetHUD(int score, int mag, int ammoTotal, string health) {
		scoreText.text = "Score: " + score;
		ammoText.text = "Ammo: " + mag + "/" + ammoTotal;
		healthText.text = "Health: " + health;
	}
	
	public void ActivateCombatHud(bool active) {
		ammoText.gameObject.SetActive(active);
		healthText.gameObject.SetActive(active);
	}
	
	public void ActivateAmmo(bool active) {
		ammoText.gameObject.SetActive(active);
	}
	
	IEnumerator TextSequence() {
		
		string[] dialogue = currentTextSequence;
		float[] timing = currentTimeSequence;
		
		Color targetCol = Color.white;
		int i = 0;
		dialogueText.color = Color.clear;
		float fadePercent = 0;
		float fadeSpeed = 2;
		//showingTextSequence = true;
		
		while(i<dialogue.Length) {
			//print (dialogue[i]);
			dialogueText.text = dialogue[i];
			dialogueText.color = targetCol;
			/*
			fadePercent = 2;
			while (fadePercent > 0) {
				fadePercent -= Time.deltaTime * fadeSpeed;
				dialogueText.color = Color.Lerp(targetCol,Color.clear,fadePercent);
				yield return null;
			}
			*/
			yield return new WaitForSeconds(timing[i]);
			
			/*
			fadePercent = 0;
			while (fadePercent < 1) {
				fadePercent += Time.deltaTime * fadeSpeed;
				dialogueText.color = Color.Lerp(targetCol,Color.clear,fadePercent);
				yield return null;
			}

			*/
			dialogueText.color = Color.clear;
			yield return new WaitForSeconds(.35f);
			i++;
		}
		showingTextSequence = false;
		
		if (sequenceIndex >= 0) {
			Player.DialogFinished(sequenceIndex);
		}
		
		
	}
	
	public void FinishObjective() {
		objectives.SetActive(false);
	}
	
	public static void ForceShowSingleMessage(string message, float time) {
		instance.ForceShowSingleMessageLocal(message,time);
		
		
	}
	
	public void ForceShowSingleMessageLocal(string message, float time) {
		if (!showingTextSequence) {
			currentTextSequence = new string[1];
			currentTimeSequence = new float[1];
			currentTextSequence[0] = message;
			currentTimeSequence[0] = time;
			
			sequenceIndex = -100;
			StopCoroutine("TextSequence");
			showingTextSequence = true;
			currentTextSequence[0] = message;
			currentTimeSequence[0] = time;
			StartCoroutine("TextSequence");
		}
	}
	
	
	
	public void ForceShowMessage(string[] message, float[] time, int index) {
		StopCoroutine("TextSequence");
		showingTextSequence = true;
		sequenceIndex = index;
		currentTextSequence = message;
		currentTimeSequence = time;
		StartCoroutine("TextSequence");
	}
	
	
	public static void ShowMessage(string message, float time) {
		
		if (!instance.showingTextSequence) {
			instance.sequenceIndex = -100;
			instance.currentTextSequence = new string[1];
			instance.currentTextSequence[0] = message;
			instance.currentTimeSequence = new float[1];
			instance.currentTimeSequence[0] = time;
			
			instance.StartCoroutine("TextSequence");
		}
	}
	
	
	
	public void StartSaladSequence() {
		ForceShowMessage(makeSaladText,makeSaladTimes,1);
	}
	
	public void StartAxeSequence() {
		//sequenceIndex = 2;
		ForceShowMessage(fetchAxeText,fetchAxeTimes,2);
	}
	public void StartPigKilledSequence() {
		//sequenceIndex = 2;
		ForceShowMessage(pigKillText,pigKillTimes,3);
	}
	
	public void StartPortalSequence() {
		//sequenceIndex = 2;
		ForceShowMessage(portalText,portalTimes,4);
	}
	public void StartFinishThemSequence() {
		ForceShowMessage(finishThemText,finishThemTime,5);
	}
	public void StartEnemyVanquishedSequence() {
		ForceShowMessage(victoryText,victoryTimes,6);
	}
}