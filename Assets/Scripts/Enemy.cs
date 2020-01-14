using UnityEngine;
using System.Collections;

public class Enemy : Character {

	public LayerMask targetObstructionMask;
	private bool wantsPortal;
	public EquipItem[] guns;

	public FauxRagdoll ragdoll;

	public float walkSpeed = 6;
	public float runSpeed = 12;
	public float waypointStep = 2f;
	public float stopThreshold = 1;
	public Transform waypointMarker;

	private Transform target;
	private Player player;
	public LayerMask pathfindingMask;

	Transform portalToFriendlyWorld;
	Transform portalToHostileWorld;

	Vector3 moveAmount;
	Vector3 planetCentre;

	private Vector3 targetLook;
	private bool targetingPlayer;

	private float currentSpeed;
	private float targetSpeed;
	private Vector3 waypoint;
	private float lastShootTime;
	private float delayBetweenShootings = 4;
	private bool shooting;
	float shootTime;
	float shootDuration;
	bool isAggro;
	float lastWaypointTime;
	private float turnSpeed = 4.5f;
	
	Vector3 aimError;
	float lastJumpTime;

	bool intro;
	bool introNextWaypoint;
	Transform introTarget;

	bool guard;
	Transform home;
	bool guarding;
	

	public Transform BaconHealthDrop;

	public static bool gameOver;

	public override void Awake ()
	{
		base.Awake ();
		//home = transform.position;
		if (waypointMarker)
			waypointMarker.gameObject.SetActive(false);
		GameObject[] p = GameObject.FindGameObjectsWithTag("Portal");
		for (int i = 0; i < p.Length; i ++) {
			if (p[i].GetComponent<Portal>().toFriendly) {
				portalToFriendlyWorld = p[i].transform;
			}
			else {
				portalToHostileWorld = p[i].transform;
			}

		}

		if ( GameObject.FindGameObjectWithTag("Player")) 
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		SetNextWaypoint();
		EquipItem(guns[Random.Range(0,guns.Length)]);

		StartCoroutine("Attack");
		StartCoroutine("AimDefficiency");
	}

	public void SetGuard(bool isG, Transform o) {
		guard = isG;
		wantsPortal = !isG;
		if (!isG) {
			home = o;
		}
		else {
			target = portalToFriendlyWorld;
			SetNextWaypoint();
		}
	}

	public void IsIntro(Transform t) {
		EquipItem(guns[0]);
		intro = true;
		introTarget = t;
		wantsPortal = true;
		target = portalToFriendlyWorld;
	//	print (target);
		SetNextWaypoint();
	}

	public void NotIntro() {
		Invoke("EndIntro",.5f);
	}

	void EndIntro() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		intro = false;
		SetNextWaypoint();
	}

	IEnumerator AimDefficiency() {
		while(true) {
		
			if (shooting) {
				yield return new WaitForSeconds(.3f);
				aimError = Random.insideUnitCircle * .0f;
				if (target) {
					Vector3 targetAim = Quaternion.LookRotation((target.position + aimError) - equippedItem.transform.position).eulerAngles;
					equippedItem.transform.eulerAngles = targetAim;
					Vector3 local = equippedItem.transform.localEulerAngles;
				}
				//equippedItem.transform.localEulerAngles = new Vector3(Mathf.Clamp(local.x,-10,10),Mathf.Clamp(local.y,-10,10),Mathf.Clamp(local.z,-10,10));
			}
			else {

				equippedItem.transform.localEulerAngles = Vector3.zero;
				yield return new WaitForSeconds(1.5f);
			}
		}
		yield return null;
	}

	public void SetStrength(float newVul,float newHealth) {
		vulnerability = newVul;
		health = newHealth;
	}

	IEnumerator Attack() {
		while (true) {
			if (equippedItem && targetingPlayer) {
				if (equippedItem.isRanged) {
					yield return new WaitForSeconds(1);
					if (lastShootTime < Time.time - delayBetweenShootings) {
						if (target) {
							Ray ray = new Ray(hands.position,(target.position- hands.position).normalized);
							RaycastHit hit;
						//	Debug.DrawRay(ray.origin,ray.direction * 30,Color.red,5);
							if (Physics.Raycast(ray,out hit,30,targetObstructionMask)) {

								if (hit.collider.tag == "Player") {
									shooting = true;
									targetSpeed = 0;
									shootDuration = Random.Range(3,5f);
									
								}
							}
						}
						else {
							if (guard) {
								targetingPlayer = false;
							}
						}
					}

				}
				else {
					yield return new WaitForSeconds(2);
				}
			}
			else {
				yield return new WaitForSeconds(2);
			}

			yield return null;
		}
	}

	void Update() {

		if (gameOver) {
			TakeDamage(5000,true);
		}

		if (guard && target == home) {
			guarding = true;
			targetSpeed = 0;
		}
		else {
			guarding = false;
		}

		currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 3);
		anim.SetFloat("Speed", currentSpeed/runSpeed);
		//transform.LookAt(player.transform);
		moveAmount = (waypoint - transform.position).normalized * currentSpeed;
		float dst = Vector3.Distance(transform.position,waypoint);

		if (Vector3.Distance(transform.position,waypoint) <= stopThreshold) {
			SetNextWaypoint();
		}

		if (shooting) {

		
			if (target) {
				targetLook = target.position;
				shootTime += Time.deltaTime;

				equippedItem.LeftMouseDown();
				equippedItem.LeftMouseHold();

				if (shootTime > shootDuration) {
					targetSpeed = ((isAggro)?runSpeed:walkSpeed);
					shooting = false;
					shootTime = 0;
					lastShootTime = Time.time;
				}
			}
			else {
				shooting = false;
			}

		}
		else {
			lastWaypointTime += Time.deltaTime;
			if (lastWaypointTime > 3 && Time.time - 10 > lastJumpTime) {
				lastJumpTime = Time.time;
				GetComponent<Rigidbody>().AddForce(transform.up * 300);
			}
			if (lastWaypointTime > 5) {
				SetNextWaypoint();
			}
		}

		Quaternion targetRot = Quaternion.LookRotation(targetLook- transform.position,transform.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
	}

	void FixedUpdate() {
		if (!shooting && !guarding) {
			//print (moveAmount);
			GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + moveAmount * Time.deltaTime);
		}
	}

	void SetNextWaypoint() {
		lastWaypointTime = 0;
		if (player) {
			if (inFriendlyWorld == player.inFriendlyWorld) {
				target = player.transform;
				targetingPlayer = true;
				wantsPortal = false;
			}
			else {
				if (guard) {
					target = home;
				}
				else {
					targetingPlayer = false;
					wantsPortal = true;
					if (inFriendlyWorld) {
						target = portalToHostileWorld;
					}
					else
						target = portalToFriendlyWorld;
				}
			}

		}
		if (target) {
			Vector3 targetOffset = ((targetingPlayer)?target.forward * -5: Vector3.zero);
			Vector3 directionToTarget = ((target.position + targetOffset) - transform.position).normalized * waypointStep;
			Vector3 waypointDirectionFromCentre = (transform.position + directionToTarget - planetCentre).normalized;
			
			Ray ray = new Ray(planetCentre + waypointDirectionFromCentre * 50,-waypointDirectionFromCentre);
			//Debug.DrawRay(ray.origin,ray.direction * 50,Color.red,1);
			RaycastHit hit;
			
			if (Physics.Raycast(ray,out hit,50,pathfindingMask)) {
				waypoint = new Vector3(hit.point.x,hit.point.y,hit.point.z);
				targetLook = waypoint + -ray.direction * .5f;
				//transform.LookAt(targetLook);
			}
			
			targetSpeed = ((isAggro)?runSpeed:walkSpeed);
			
			waypointMarker.position = targetLook;
		}
	}

	public override void OnTriggerEnter (Collider c)
	{
		//print (intro + "AAAA " + target.name);
		if (wantsPortal || intro) {
			base.OnTriggerEnter (c);
			if (c.tag == "Portal") {
				if (intro) {
					SequenceManager.EnteredPortalIntro();
					wantsPortal = false;
					introNextWaypoint = true;
					target = introTarget;
				}
			}


		}
	}


	public override void UpdateGravity ()
	{
		base.UpdateGravity ();

		planetCentre = myGravity.attractor.transform.position;
		SetNextWaypoint();
		if (equippedItem) {
			equippedItem.ChangeGravity(myGravity.attractor);
		}
	}

	public override void TakeDamage (float dmg, bool p)
	{
		base.TakeDamage (dmg,p);
		anim.SetTrigger("Hurt");
		isAggro = true;
		shooting = true;
		shootDuration = Random.Range(5,7f);
	}

	public override void Die ()
	{

		if (!dead) {
			if(equippedItem) {
				equippedItem.Die(myGravity.attractor.transform.position);
			}
			FauxRagdoll rag = Instantiate(ragdoll,transform.position,transform.rotation) as FauxRagdoll;
			rag.SetGrav(myGravity.attractor.transform.position);
			base.Die ();

			if(Random.value<0.33f) {
				Transform spawn = Instantiate(BaconHealthDrop,transform.position,transform.rotation) as Transform;
			}

			EnemySpawnManager.EnemyDead();
			Destroy(gameObject);
		}
	}
	
}
