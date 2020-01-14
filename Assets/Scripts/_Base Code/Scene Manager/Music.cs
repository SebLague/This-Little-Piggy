using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class Music : MyBase {

	private AudioClip[] playlist;

	private int trackIndex;
	private bool trackIsPlaying;

	public struct TrackOrder {
		public AudioClip track;
		public int orderIndex;
	}

	public override void Awake() {
		base.Awake();
		DontDestroyOnLoad(this.gameObject);
		isMusicPlayer = true;
	}

	public void SetPlaylist(AudioClip[] newPlaylist, bool randomizeOrder, bool finishCurrentTrack) {
		// Randomize order of playlist
		if (newPlaylist != null && newPlaylist.Length > 0) {
			if (randomizeOrder) {
				playlist = new AudioClip[newPlaylist.Length];

				for (int i = 0; i < newPlaylist.Length; i ++) {
					int j = Random.Range(0,i+1);
					if (j!=i)
						playlist[i] = playlist[j];
					playlist[j] = newPlaylist[i];
				}
			}
			else
				playlist = newPlaylist;

			// Start playing new playlist (either immediately or after current track)
			trackIndex = 0;

			if (!finishCurrentTrack){
				StopCoroutine("PlayTracks");
				StartCoroutine("PlayTracks");
			}

			if (!trackIsPlaying)
				StartCoroutine("PlayTracks");
		}
	}

	// Automatically switch to next track in playlist once current track has finished playing
	// Note: not using "yield return new WaitForSeconds(x)" as this will not work in pause menus where timescale = 0
	IEnumerator PlayTracks() {
		trackIsPlaying = true;
		float trackEndTime = 0;
		while (true) {
			if (trackEndTime <= 0) {
				PlayAudio(playlist[trackIndex]);
				trackEndTime = playlist[trackIndex].length;

				trackIndex++;
				trackIndex %= playlist.Length;
				yield return null;
			}
			else {
				trackEndTime -= RealTime.deltaTime;
				yield return null;
			}

		}
	}

}
