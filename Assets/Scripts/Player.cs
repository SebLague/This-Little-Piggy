using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public class Player : Character {

	public EquipItem testItem;

	public TextMesh scoreText;
	private bool inPickup;
	private Pickup currentPickup;
	private PlayerController controller;
	private ObscuredFloat score;
	public HUD hud;
	private bool pigKilled;

	int priorEnemiesKilled;

	public EquipItem axe;

	public static Player instance;

	int objectiveIndex = -1;
	private int targetPotatoCount = 3;
	private int targetEggCount = 1;
	private int targetOnionCount = 2;

	private int currentPotatoCount;
	private int currentEggCount;
	private int currentOnionCount;
	
	private int saladPreparedCount;

	public Pickup emptyPlate;
	public Pickup potatoSaladPlate;

	private int currentAxeCount;
	private int currentPigCount;

	private int targetEnemyCount;
	private int targetIdolCount = 5;
	private int currentEnemyCount;
	private int currentIdolCount;

	public GameObject axes;

	private float multiplier = 1;
	private bool fighting;

	public AudioClip teleport;
	public Transform pickupArea;
	public LayerMask pickMask;


	public override void Awake ()
	{
		volumeDiv = 3.5f;
		vulnerability = .25f;
	
		controller = GetComponent<PlayerController>();
		base.Awake ();
		axes.SetActive(false);
		hud.ActivateCombatHud(false);
		instance = this;
		isPlayer = true;
		targetEnemyCount = EnemySpawnManager.totalEnemiesRemaining;
		targetIdolCount = EnemySpawnManager.totalIdolsRemaining;

		if (testItem)
			EquipItem(testItem);
	
	}

	public static void AddScore(float addScore) {
		instance.AffectMultiplier(.5f);
		instance.score += addScore * instance.multiplier;
	}

	public static void EnemyDied() {
		if (instance.objectiveIndex == 5) {
			instance.currentEnemyCount++;
			instance.UpdateFinishObjective();
		}
		else {
			instance.priorEnemiesKilled ++;
		}
		AddScore(100);
		AffectPlayerMultiplier(1);
	}

	public static void IdolDestroyed() {
		AffectPlayerMultiplier(1.5f);
		AddScore(150);
		instance.currentIdolCount++;
		instance.UpdateEnemyObjective();

	}

	void Update () {
		if (fighting) {
			health += 1 *Time.deltaTime;
		}
		AffectMultiplier(-1f/5f * Time.deltaTime);
		UpdateHud();
		if (equippedItem) {
			if (Input.GetMouseButtonDown(0)) {
			//	StartEnemy();
				equippedItem.LeftMouseDown();
			}
			else if (Input.GetMouseButtonUp(0)) {
				equippedItem.LeftMouseUp();
			}
			else if (Input.GetMouseButton(0)) {
				equippedItem.LeftMouseHold();
			}

			if (Input.GetButtonDown("Reload")) {
				equippedItem.Reload();
			}

			if (equippedItem.isGun) {
				if (equippedItem.ammoTotalCounter <= 0) {
					EquipItem(axe);
				}
			}

		}
		if (Input.GetButtonDown("Action") && !inPickup) {
			foreach (Collider c in Physics.OverlapSphere(pickupArea.position,2,pickMask)) {
				
				if (c.tag == "Pickup") {
					//print(c.gameObject.name);
					inPickup = true;
					currentPickup = c.gameObject.GetComponent<Pickup>();
					//print (inPickup);
					break;
				}
			}
		}

		if (inPickup) {
			//print ("SOGO");
			if (Input.GetButtonDown("Action") || currentPickup.action == Pickup.Action.AutoPickup) {
				if (!currentPickup.disabled) {
					if (currentPickup.action == Pickup.Action.Equip || currentPickup.action == Pickup.Action.AutoPickup) {
						EquipItem(currentPickup.equipItem);
						PickupItem(currentPickup.name);
					}
					else
						PickupItem(currentPickup.name);
					//print (currentPickup.name);
					currentPickup.Used();
					inPickup = false;

				
				}
			}
		}
	}


	public override void EquipItem (EquipItem item)
	{
		base.EquipItem (item);
		equippedItem.isPlayerWeapon = true;

		if (equippedItem.isGun) {
			hud.ActivateAmmo(true);

		}
		else
			hud.ActivateAmmo(false);
	}

	public override void OnTriggerEnter(Collider c) {
		base.OnTriggerEnter(c);
		if (c.tag == "Portal") {
			PlayAudio(teleport);
		}
		if (c.tag == "Pickup") {

			inPickup = true;
			currentPickup = c.GetComponent<Pickup>();
		}
	}
	
	public override void OnTriggerExit(Collider c) {
		base.OnTriggerExit(c);
		if (c.tag == "Pickup") {
			inPickup = false;
		}
	}

	public override void UpdateGravity ()
	{
		base.UpdateGravity ();
		controller.gravCentre = myGravity.attractor.transform.position;
		if (equippedItem) {
			equippedItem.ChangeGravity(myGravity.attractor);
		}
	}

	public void PickupItem(string id) {
		id = id.ToLower();
		switch (id) {
		case "potato":
			score += 10;
			currentPotatoCount ++;
			break;
		case "egg":
			score += 5;
			currentEggCount ++;
			break;
		case "onion":
			score += 2;
			currentOnionCount ++;
			break;
		case "plate":
			if (objectiveIndex == 1) {
				score += 100;
				saladPreparedCount = 1;
				UpdateSecondObjective();

				potatoSaladPlate.gameObject.SetActive(true);
			}

			break;
		case "axe":
			currentAxeCount++;
			if (objectiveIndex == 2) {
				UpdateThirdObjective();
			}
			break;
		case "salad":
			//Debug.Break();
			Application.LoadLevel("Win");
			break;
		default:
			break;
		}

		if (objectiveIndex == 0) {
			UpdateFirstObjective();

			if (currentEggCount >= targetEggCount && currentPotatoCount >= targetPotatoCount && currentOnionCount >= targetOnionCount) {
				objectiveIndex = -100;
				hud.FinishObjective();
				hud.StartSaladSequence();
			}
		}

	
	}


	public void UpdateHud() {
		float healthD = health;
		string healthString = ((int)health).ToString();
		if (equippedItem) {
			hud.SetHUD((int)score,equippedItem.ammoMagCounter,equippedItem.ammoTotalCounter,healthString);
		}
		else {

			hud.SetHUD((int)score,0,0,healthString);
		}

	}

	public void PigKilled() {
		if (!pigKilled) {
			currentPigCount ++;
			if (objectiveIndex == 2) {
				UpdateThirdObjective();
			}
			pigKilled = true;


		}
	}

	void StartEnemy() {
		fighting = true;

		SequenceManager.ActivatePortal();
	}


	public override void TakeDamage (float dmg,bool p)
	{
		base.TakeDamage (dmg,p);
		AffectMultiplier(-.2f);
		//print (health);
	}

	/*
	 * Objectives
	 */

	// Trigger objective start
	public static void DialogFinished(int index) {
		instance.objectiveIndex = index;
		switch(index) {
		case 0:
			instance.StartFirstObjective();
			break;
		case 1:
			instance.StartSecondObjective();
			break;
		case 2:
			instance.StartThirdObjective();
			break;
		case 3:
			instance.hud.StartPortalSequence();
			instance.StartEnemy();
			break;
		case 4:
			instance.StartEnemyObjective();
			break;
		case 5:
			EnemySpawnManager.Finish();
			instance.StartFinishThemObjective();
			break;
		case 6:
			instance.GameWinReady();
			break;
		}
	}

	// Start objectives
	
	void StartFirstObjective() {
		hud.SetNewObjective("Potatoes","Eggs","Onions");
		UpdateFirstObjective();
	}
	
	void StartSecondObjective() {
		emptyPlate.disabled = false;
		hud.SetNewObjective("Prepare salad");
		UpdateSecondObjective();
	}
	
	void StartThirdObjective() {
		axes.SetActive(true);
		hud.SetNewObjective("Find axe", "Kill pig");
		UpdateThirdObjective();
	}

	public void StartEnemyObjective() {
		hud.SetNewObjective("Destroy idols");
		UpdateEnemyObjective();
	}

	void StartFinishThemObjective() {
		hud.SetNewObjective("Kill invaders");
		UpdateFinishObjective();
	}

	// Update objectives

	public void UpdateFirstObjective() {
		if (objectiveIndex == 0)
			hud.UpdateObjective(currentPotatoCount,targetPotatoCount,currentEggCount,targetEggCount,currentOnionCount,targetOnionCount);
	}
	
	public void UpdateSecondObjective() {
		if (objectiveIndex == 1)
			hud.UpdateObjective(saladPreparedCount,1);
		if (saladPreparedCount == 1) {
			objectiveIndex = -100;
			hud.FinishObjective();
			hud.StartAxeSequence();
		}
	}
	
	public void UpdateThirdObjective() {
		if (objectiveIndex == 2)
			hud.UpdateObjective(currentAxeCount,1,currentPigCount,1);
		if (currentPigCount >= 1) {
			hud.FinishObjective();
			hud.StartPigKilledSequence();
			SequenceManager.Warn();
		}
	}

	
	public void UpdateEnemyObjective() {
		if (objectiveIndex == 4)  {
			hud.UpdateObjective(currentIdolCount,targetIdolCount);
			if (currentIdolCount >= targetIdolCount) {
				hud.FinishObjective();
				hud.StartFinishThemSequence();
				objectiveIndex = -100;
			}
		}
	}

	public void UpdateFinishObjective() {
		if (objectiveIndex == 5) {
			hud.UpdateObjective(currentEnemyCount + priorEnemiesKilled,targetEnemyCount + priorEnemiesKilled);
		
			if (currentEnemyCount >= targetEnemyCount) {
				hud.FinishObjective();
				hud.StartEnemyVanquishedSequence();
				Enemy.gameOver = true;
				objectiveIndex = -100;

			}
		}
	}


	public static void FightBegins() {
		instance.hud.ActivateCombatHud(true);
	}

	void AffectMultiplier(float n) {
		multiplier = Mathf.Clamp(multiplier + n,1,Mathf.Infinity);
	}

	public static void AffectPlayerMultiplier(float n) {
		instance.AffectMultiplier(n);
	}

	public void GameWinReady() {
		potatoSaladPlate.disabled = false;
		hud.SetNewObjective("Eat potato salad");
	}

	public override void Die ()
	{
		base.Die ();
		Application.LoadLevel("Lose");
	}



 
}
