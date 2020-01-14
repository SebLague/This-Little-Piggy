using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public static class PlayerScoreInfo {

	public static string username = "Username";
	public static ObscuredInt highscore;

	public static bool pendingHighscoreUpload;
	public static string pendingUploadUsername;
	public static ObscuredInt pendingUploadScore;

	public static ObscuredInt currentScore; // a place to store player score between scenes

	// Load values from PlayerPrefs
	public static void Init() {
		username = ObscuredPrefs.GetString("username", "My Name");
		highscore = ObscuredPrefs.GetInt("highscore");

		bool.TryParse(ObscuredPrefs.GetString("pendingUpload"), out pendingHighscoreUpload);
		pendingUploadUsername = ObscuredPrefs.GetString("pendingUsername");
		pendingUploadScore = ObscuredPrefs.GetInt("pendingUploadScore");
	}

	// Set new highscore (if new score is higher)
	public static void AddScore(int score) {
		if (score > highscore) {
			highscore = score;
			ObscuredPrefs.SetInt("highscore", highscore);
		}
	}

	// Keeps track of highest score that has failed to be uploaded to highscore database in order to try again later.
	public static void FailedToUploadHighscore(int score) {
		if (score >= pendingUploadScore) {
			pendingHighscoreUpload = true;
			pendingUploadUsername = username;
			pendingUploadScore = score;

			ObscuredPrefs.SetString("pendingUpload",pendingHighscoreUpload.ToString());
			ObscuredPrefs.SetString("pendingUsername", pendingUploadUsername);
			ObscuredPrefs.SetInt("pendingHighscore", pendingUploadScore);
		}
	}

	// Pending highscore upload has successfully been uploaded and can now be cleared
	public static void ClearPendingHighscoreUpload() {
		pendingHighscoreUpload = false;
		ObscuredPrefs.SetString("pendingUpload",pendingHighscoreUpload.ToString());
	}

	// Set new username
	public static void SetUsername(string newUsername) {
		username = newUsername;
		ObscuredPrefs.SetString("username", newUsername);
	}
}
