using UnityEngine;
using System.Collections;

/*
 * This script should be attached to an empty gameobject in every scene.
 */

public class SceneMusicManager : MonoBehaviour {
	
	public enum MusicDirection {ContinuePlaylist, StartNewPlaylistImmediate, StartNewPlaylistAfterCurrentTrack};
	public MusicDirection musicDirection;

	public AudioClip[] trackList;
	private Music music;
	public AudioClip warSong;

	private static SceneMusicManager instance;


	void Awake() {
		instance = this;
		if (GameObject.FindGameObjectWithTag("Music")) {
			music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>();
			music.myTransform.parent = transform;
		}
		else {
			music = new GameObject("Music", typeof(Music)).GetComponent<Music>();
			music.tag = "Music";
			music.myTransform.parent = transform;
		}

		switch (musicDirection) {
		case MusicDirection.StartNewPlaylistImmediate:
			music.SetPlaylist(trackList,true,false);
			break;
		case MusicDirection.StartNewPlaylistAfterCurrentTrack:
			music.SetPlaylist(trackList,true,true);
			break;

		}
	}

	public static void WarTrack() {
		instance.trackList = new AudioClip[1];
		instance.trackList[0] = instance.warSong;
		instance.music.SetPlaylist(instance.trackList,false,false);
	}

	void OnDestroy() {
		//music.myTransform.parent = null;
	}
	
}
