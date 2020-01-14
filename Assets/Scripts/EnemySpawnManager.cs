using UnityEngine;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour {

	public static int totalEnemiesRemaining = 20;
	public static int totalIdolsRemaining = 4;
	public static int liveEnemyCount;
	

	int totalEnemyCount = 1;
	int totalIdolCount = 1;
	private int enemiesSpawnedCount;


	private float minHealth = 20;
	private float maxHealth = 20;
	private float healthBuffPercent;

	private float vulnerabilityMultiplierMax = 10;
	private float vulnerabilityPercent;


	public Enemy enemy;
	public Transform[] spawns;
	public Transform[] introSpawns;

	public Transform[] guardSpawns;

	public Enemy[] introEnemies;
	public Transform[] introTargets;

	private Enemy[] guards;

	private static EnemySpawnManager instance;

	private float minSpawn = 5f;
	private float maxSPawn = 17f;

	public static void Finish() {
		instance.minSpawn = 2f;
		instance.maxSPawn = 5f;
	}

	// Use this for initialization
	void Awake () {
		instance = this;
		totalEnemyCount = totalEnemiesRemaining;
		totalIdolCount = totalIdolsRemaining;

		guards = new Enemy[guardSpawns.Length];

		for (int i =0; i <guardSpawns.Length; i ++) {
			liveEnemyCount ++;
			if (totalIdolsRemaining<=0) {
				enemiesSpawnedCount ++;
			}
			Transform spawn = spawns[Random.Range(0,spawns.Length)];
			Enemy e = Instantiate(enemy,guardSpawns[i].position,guardSpawns[i].rotation) as Enemy;
			e.SetGuard(true,guardSpawns[i]);
			e.SetStrength(1,20);
			guards[i] = e;
		}
	}
	
	IEnumerator Spawn() {
		while (!Enemy.gameOver) {
			if (liveEnemyCount <= 10) {

				vulnerabilityPercent = 1f-((float)totalIdolsRemaining/(float)totalIdolCount);
				float enemyDepletedPercent = 1f-(float)totalEnemiesRemaining/(float)totalEnemyCount;
				float health = Mathf.Lerp(minHealth,maxHealth,enemyDepletedPercent);
				float vulnerability = Mathf.Lerp(1,vulnerabilityMultiplierMax,vulnerabilityPercent);

				liveEnemyCount ++;
				if (totalIdolsRemaining<=0) {
					enemiesSpawnedCount ++;
				}
				Transform spawn = spawns[Random.Range(0,spawns.Length)];
				Enemy e = Instantiate(enemy,spawn.position,spawn.rotation) as Enemy;
				e.SetStrength(vulnerability,health);
			}
			yield return new WaitForSeconds (Random.Range(minSpawn,maxSPawn));
		}
		for (int i =0; i <guardSpawns.Length; i ++) {
			if (guards[i] != null) {
				guards[i].SetGuard(false,null);
			}
		}
	}

	public void IntroSpawns() {
		int i =0;
		introEnemies = new Enemy[introSpawns.Length];
		foreach (Transform t in introSpawns) {
			if (totalIdolsRemaining<=0) {
				enemiesSpawnedCount ++;
			}
			liveEnemyCount ++;
			introEnemies[i] = Instantiate(enemy,t.position,t.rotation) as Enemy;
			introEnemies[i].IsIntro(introTargets[i]);
			i++;
		}
	}

	public void StartSpawning() {
		foreach (Enemy e in introEnemies) {
			e.NotIntro();
		}
		StartCoroutine("Spawn");
	}

	public static void IdolDestroyed() {
		totalIdolsRemaining --;
		Player.IdolDestroyed();
	}

	public static void EnemyDead() {
		totalEnemiesRemaining--;
		liveEnemyCount--;
		Player.EnemyDied();
	}
}
