using UnityEngine;
using System.Collections;

public class AddHighscore : MonoBehaviour {

	public delegate void HighscoreUploaded(bool successful);
	public HighscoreUploaded uploadComplete;

	public void AddScore(string username, int score) {
		username += "+" + GetRandom();
		StartCoroutine(UploadScore(username, score));
	}

	// Upload score to server
	IEnumerator UploadScore(string username, int score){
		WWW www = new WWW(Highscores.webURL + Highscores.privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
		yield return www;
		// An error occurred
		if (!string.IsNullOrEmpty(www.error)) {
			print(www.error);
			PlayerScoreInfo.FailedToUploadHighscore(score);
			if (uploadComplete != null)
				uploadComplete(false);
		}
		else {
			if (uploadComplete != null)
				uploadComplete(true);
		}


	}

	// Get random string (appended to username to prevent duplicate username overwriting)
	string GetRandom() {
		string random = System.DateTime.Now.Date.ToShortDateString() + System.DateTime.Now.TimeOfDay.ToString();
		random = random.Replace('.', '0');
		random = random.Replace(' ', '0');
		random = random.Replace(':', '0');
		random = random.Replace('/', '0');
		return random;
	}
}
