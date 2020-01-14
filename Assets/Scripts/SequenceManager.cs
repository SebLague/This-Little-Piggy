using UnityEngine;
using System.Collections;

public class SequenceManager : MyBase {

	public GameObject portalCam1;
	public GameObject portalCam2;

	public EnemySpawnManager spawnEnemy;
	public Transform portal;
	private static SequenceManager instance;
	public GameObject player;
	public IntroCamera cam;
	int introEnterCount;
	public AudioClip portalWarning;
	public AudioClip portalArrive;

	public override void Awake() {
		base.Awake();
		portal.gameObject.SetActive(false);
		cam.gameObject.SetActive(false);
		instance = this;

	
	}

	public static void Warn() {
		instance.PlayAudio(instance.portalWarning);
	}

	public static void ActivatePortal() {

		instance.player.SetActive(false);
		instance.cam.gameObject.SetActive(true);
		instance.StartCoroutine("PortalArrive");
		instance.cam.ActivatePortalIntro();
	}

	IEnumerator PortalArrive() {
		SceneMusicManager.WarTrack();
		PlayAudio(portalArrive);
		portal.gameObject.SetActive(true);
		portalCam2.SetActive(true);
		float scaleV = 0;
		float scale = 0;
		spawnEnemy.IntroSpawns();
		while (scale < 1) {
			scale = Mathf.MoveTowards(scale,1,1 * Time.deltaTime);
			//scale = Mathf.Clamp01(scale);
			portal.localScale = Vector3.one * scale;
			yield return null;
		}



	}

	public static void EnteredPortalIntro() {
		instance.introEnterCount ++;
		if (instance.introEnterCount >= 2) {
			instance.Invoke("ResumeGameplay",2);
		}
	}

	void ResumeGameplay() {
		Portal.activateCams = true;
		portalCam1.SetActive(false);
		portalCam2.SetActive(false);
		cam.gameObject.SetActive(false);
		player.SetActive(true);
		Player.FightBegins();
		spawnEnemy.StartSpawning();
	}
}
