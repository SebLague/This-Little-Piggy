using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AddHighscore))]
[RequireComponent (typeof (Highscores))]
public class DisplayHighscores : MonoBehaviour {

	public int displayCount = 10;

	private Highscores highscores;
	private AddHighscore addHighscores;

	void Awake() {
		highscores = GetComponent<Highscores>();
		addHighscores = GetComponent<AddHighscore>();

		//addHighscores.AddScore("Bob", 10);

		if (PlayerScoreInfo.pendingHighscoreUpload) {
			addHighscores.uploadComplete = UploadedPendingScores;
			addHighscores.AddScore(PlayerScoreInfo.pendingUploadUsername, PlayerScoreInfo.pendingUploadScore);
		}

		highscores.highscoresRetrieved = OnHighscoresFetched;

		StartCoroutine("RefreshHighscores");

	}
	

	// Pending scores have been uploaded
	void UploadedPendingScores(bool uploadSuccessful) {
		if (uploadSuccessful) {
			PlayerScoreInfo.ClearPendingHighscoreUpload();
			highscores.GetScores(displayCount);
		}
	}

	// Highscores have been fetched
	void OnHighscoresFetched(bool downloadSuccessful) {
		print (downloadSuccessful + "");
		if (downloadSuccessful) {
			for (int i = 0; i < highscores.highscoreUsernames.Length; i ++) {
				print ((i+1) + ". " + highscores.highscoreUsernames[i] + ": " + highscores.highscoreScores[i]);
			}
		}
		else {
			print ("Failed to connect to online highscores");
		}

		StartCoroutine("RefreshHighscores");
	}

	void OnDestroy() {
		addHighscores.uploadComplete = null;
		highscores.highscoresRetrieved = null;
	}

	IEnumerator RefreshHighscores() {
		highscores.GetScores(displayCount,Highscores.SortType.LowToHigh);
		yield return new WaitForSeconds(15);
	}


}
