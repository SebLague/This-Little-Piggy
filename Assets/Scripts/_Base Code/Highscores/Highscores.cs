using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Highscores : MonoBehaviour {

	/*
	 * This class handles the fetching and formatting of scores from the dreamlo database
	 * Uploading of highscores is handled by the AddHighscore class
	 */

	// Database access information
	public const string  privateCode = "1vLmjH3Toka4ourCNhLqlAyNbzsFx6w068AEWBvymQcg";
	public const string publicCode = "53e4fa966e51b40ec4feb5f6";
	public const string webURL = "http://dreamlo.com/lb/";
	// Web address: http://dreamlo.com/lb/1vLmjH3Toka4ourCNhLqlAyNbzsFx6w068AEWBvymQcg

	// System
	public enum SortType {HighToLow, LowToHigh};
	private SortType sortType;

	public delegate void HighscoresRetrieved(bool downloadSuccessful);
	public HighscoresRetrieved highscoresRetrieved;

	private string highscoreTextStream;


	// Formatted data:
	[HideInInspector]
	public int[] highscoreScores;
	[HideInInspector]
	public string[] highscoreUsernames;

	[HideInInspector]
	public bool myHighscoreRankIsSet;
	[HideInInspector]
	public int myHighscoreRank;



	public struct Score {
		public string username;
		public int score;
	}


	public void GetScores(int placesToDisplay, SortType sorting = SortType.HighToLow) {
		sortType = sorting;
		StartCoroutine(DownloadScoresFromServer(placesToDisplay));
	}
	
	IEnumerator DownloadScoresFromServer(int placesToDisplay)
	{
		WWW www = new WWW(webURL +  publicCode  + "/pipe");
		yield return www;
		if (string.IsNullOrEmpty(www.error)) {
			highscoreTextStream = www.text;
			FormatHighscores(placesToDisplay);
		}
		else { // And error occurred
			if (highscoresRetrieved != null)
				highscoresRetrieved(false);
		}
		
	}

	// Format highscore text received from server
	void FormatHighscores(int placesToDisplay) {

		string[] rows = highscoreTextStream.Split(new char[] {'\n'},System.StringSplitOptions.RemoveEmptyEntries); // Separate entries into rows
		placesToDisplay = Mathf.Clamp(placesToDisplay,0,rows.Length);

		Score[] allEntries = new Score[rows.Length];
		int[] allScores = new int[rows.Length];

		// Go through every highscore entry
		for (int i = 0; i <rows.Length; i ++) {
			string[] rowInfo = rows[i].Split(new char[] {'|'});

			if (rowInfo.Length > 1) {
				int score = 0;
				int.TryParse(rowInfo[1], out score);
				allScores[i] = score;

				string[] nameDecode = rowInfo[0].Split(new char[] {'+'});
				allEntries[i].username = nameDecode[0];
				allEntries[i].score = score;

			}
		}

		// Sort Highscores as low to high (high to low by default)
		if (sortType == SortType.LowToHigh) {
			SortHighToLow(ref allEntries);
			SortHighToLow(ref allScores);
		}

		// Set top highscore entries string
		highscoreUsernames = new string[allEntries.Length];
		highscoreScores = new int[allEntries.Length];
		for (int i = 0; i <allEntries.Length; i ++) {
			highscoreUsernames[i] = allEntries[i].username;
			highscoreScores[i] = allEntries[i].score;
		}

		// Get this user's position in highscore
		List<int> allScoresList = new List<int>(allScores);
		myHighscoreRank = allScoresList.IndexOf(PlayerScoreInfo.highscore);
		if (myHighscoreRank == -1) // player's top score was not found in database
			myHighscoreRankIsSet = false;
		else
			myHighscoreRankIsSet = true;

		// Retrieved Highscores
		if (highscoresRetrieved != null)
			highscoresRetrieved(true);

	}

	// Sort scores from highest to lowest
	void SortHighToLow(ref Score[] scores) {
		System.Array.Sort(scores,(x,y) => x.score.CompareTo(y.score));
	}

	void SortHighToLow(ref int[] scores) {
		System.Array.Sort(scores,(x,y) => x.CompareTo(y));
	}

	// Sort scores from lowest to highest
	void SortLowToHigh(ref Score[] scores) {
		System.Array.Sort(scores,(x,y) => x.score.CompareTo(y.score));
	}
	
	void SortLowToHigh(ref int[] scores) {
		System.Array.Sort(scores,(x,y) => x.CompareTo(y));
	}


}
