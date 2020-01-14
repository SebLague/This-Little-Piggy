using UnityEngine;
using System.Collections;

/*
 * Base class with added functionality to extend instead of MonoBehaviour
 */

public class MyBase : MonoBehaviour {

	[HideInInspector]
	public float volumeDiv = 1;
	// Audio
	[HideInInspector]
	public bool isMusicPlayer;
	private AudioSource myAudio;
	private bool hasAudio;
	
	// Cached components
	[HideInInspector]
	public Transform myTransform;
	
	// Init
	public virtual void Awake() {
		AudioManager.updateAudio += UpdateAudioLevels;
		if (!GetComponent<AudioSource>())
			gameObject.AddComponent<AudioSource>();
		myAudio = GetComponent<AudioSource>();
		if (myAudio) {
			hasAudio = true;
		}
		
		myTransform = transform;
		UpdateAudioLevels();
	}
	
	// Play an audio clip
	public void PlayAudio(AudioClip sound) {
	//	Debug.Log ("Play audio "+sound);
		if (sound!=null) {
			if (hasAudio) {
				if (isMusicPlayer) {
					myAudio.clip = sound;
					myAudio.Play();
				}
				else {
					myAudio.PlayOneShot(sound);
				}
			}
		}

	}
	
	// System methods
	#region System (private methods no one really needs to know exist)
	// Update audio levels from AudioManager
	void UpdateAudioLevels() {
		if (hasAudio) {
			myAudio.volume = ((isMusicPlayer)?AudioManager.VolumeMusic:AudioManager.VolumeSfx) * AudioManager.VolumeMaster;
			myAudio.volume /= volumeDiv;
		}
	}
	
	// Unsubscribe from delegates when destroyed
	void OnDestroy() {
		AudioManager.updateAudio -= UpdateAudioLevels;
	}
	#endregion
}
