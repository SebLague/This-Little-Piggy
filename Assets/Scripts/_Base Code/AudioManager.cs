using UnityEngine;
using System.Collections;

public static class AudioManager {

	private static float volumeSfx = 1;
	private static float volumeMusic = 1;
	private static float volumeMaster = 1;

	public delegate void UpdateAudio();
	public static UpdateAudio updateAudio;

	// Fetch volume levels from PlayerPrefs
	public static void Init() {
		VolumeSfx = PlayerPrefs.GetFloat("volumeSfx",1);
		VolumeMusic = PlayerPrefs.GetFloat("volumeMusic",1);
		VolumeMaster = PlayerPrefs.GetFloat("volumeMaster",1);
	}

	// Get set methods for all volume controls:
	#region Volume Controls
	public static float VolumeSfx {
		get { return volumeSfx;}
		set {
			volumeSfx = value;
			PlayerPrefs.SetFloat("volumeSfx", volumeSfx);
			updateAudio();
		}
	}

	public static float VolumeMusic {
		get { return volumeMusic;}
		set {
			volumeMusic = value;
			PlayerPrefs.SetFloat("volumeMusic", volumeMusic);
			updateAudio();
		}
	}

	public static float VolumeMaster {
		get { return volumeMaster;}
		set {
			volumeMaster = value;
			PlayerPrefs.SetFloat("volumeMaster", volumeMaster);
			updateAudio();
		}
	}
	#endregion
}
