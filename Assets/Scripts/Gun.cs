using UnityEngine;
using System.Collections;

public class Gun : EquipItem {
	

	// Gun attributes
	public string gunName = "My Gun";
	public enum FireMode {Automatic = 0, Semi = 1, Burst = 2};
	public FireMode fireMode;
	public float reloadTime;

	public int rpm = 300;
	public int magSize = 12;
	public int startingAmmo = 100;
	public int burstCount = 3;
	public float delayBetweenBursts = .2f;
	
	// Projectile attributes
	public enum ProjectileType {Raycast, Physical};
	public ProjectileType projectileType;

	public float impactDamage = 5;
	public float rayProjectileRange = 30;
	public float physicalProjectileVelocity = 100;
	public LayerMask collisionMask;
	
	public Transform[] projectileSpawns = new Transform[1];
	public Projectile physicalProjectile;
	
	
	
	// Audio/visual effects
	public bool useShells;
	public Transform shellEjectPoint;
	public Shell shell;
	public bool useAudio;
	public AudioClip shootAudio;
	public AudioClip reloadAudio;
	public AudioClip outOfAmmoAudio;
	public bool useMuzzleflashTextures;
	public Texture2D[] muzzleFlashTextures;
	
	
	// System
	private float timeBetweenShots;
	private float nextShootTime;
	private int availableAmmo;
	private int ammoRemainingInMag;
	private int projectilesFiredInBurst;
	private float nextBurstTime;
	private bool burstInProgress;
	private Transform gunContainer; /// Empty to hide projectiles/shells etc under for scene organisation 
	private Material[] muzzleFlashMaterials;
	
	private FauxGravityAttractor masterGravity;

	public override void Awake() {
		base.Awake();
		isGun = true;
		isRanged = true;
		allowAds = true;
		timeBetweenShots = 60f/rpm;
		
		availableAmmo = startingAmmo;
		ammoTotalCounter = availableAmmo;
		UpdateAmmoAfterReload();

		if (projectileType == ProjectileType.Physical || useShells) {
			GameObject c = new GameObject(gunName + "_Container");
			gunContainer = c.transform;
		}


		// Set shell effects
		if (!shellEjectPoint || !shell)
			useShells = false;

		
		// Set muzzleflash effects
		if (useMuzzleflashTextures) {

			if (muzzleFlashTextures.Length == 0) 
				useMuzzleflashTextures = false;
			else {
				Transform[] spawnsWithPlanes = new Transform[projectileSpawns.Length];
				int spawnsWithPlanesCount = 0;
				foreach (Transform t in projectileSpawns) {
					if (t.Find("Muzzleflash")) {
						spawnsWithPlanes[spawnsWithPlanesCount] = t;
						spawnsWithPlanesCount++;
					}
				}

				if (spawnsWithPlanesCount > 0) {
					muzzleFlashMaterials = new Material[spawnsWithPlanesCount];
					for (int i = 0; i < spawnsWithPlanes.Length; i ++) {
						muzzleFlashMaterials[i] = spawnsWithPlanes[i].Find("Muzzleflash").GetComponent<Renderer>().material;
					}
				}
				else {
					useMuzzleflashTextures = false;
				}
			}
		}
	}
	

	
	private void Shoot() {

		if (Time.time >= nextShootTime) {
			timeBetweenShots = 60f/rpm; /// Updated to allow editing value in inspector to take immediate effect
			nextShootTime = Time.time + timeBetweenShots;	
			projectilesFiredInBurst ++;

			bool canShoot = true;
			// Effects
			if (useItemHandling) {
				canShoot = handling.QueryCanShoot(); 

			}
			if (ammoRemainingInMag > 0 ) {
				if (canShoot) {
					PlayAudio(shootAudio);
					if (useMuzzleflashTextures)
						StartCoroutine("Muzzleflash");
					if (useShells) {
						Shell newShell = Instantiate (shell, shellEjectPoint.position,shellEjectPoint.rotation) as Shell;
						newShell.SetGravity(masterGravity);
						newShell.GetComponent<Rigidbody>().AddForce((shellEjectPoint.forward + new Vector3(Random.Range(-.1f,.1f),Random.value,0) * 1f) * 200);
						newShell.GetComponent<Rigidbody>().AddTorque(Vector3.up * Random.Range(50,200));
						newShell.transform.parent = gunContainer;
					}
					if (useItemHandling) {
						handling.Recoil();
					}
				

				// Shoot a projectile from each spawn point
				foreach (Transform t in projectileSpawns) {
					if (ammoRemainingInMag > 0) {
						if (isPlayerWeapon) {
							ammoRemainingInMag --;
							//ammoTotalCounter --;
							ammoMagCounter--;
						}
						FireProjectile(t);
					}
				}
				if (projectileSpawns.Length == 0)
					Debug.LogWarning("Gun has no projectile spawn point");
				}
			}
			else {
				PlayAudio(outOfAmmoAudio);
			}
		}
		if (ammoRemainingInMag == 0) {
			Reload();
		}
	}
	
	private void FireProjectile(Transform spawn) {
	
		// Either: Instantiate physical projectiles
		if (projectileType == ProjectileType.Physical) {
			if (physicalProjectile) {
				Projectile p = Instantiate(physicalProjectile,spawn.position,spawn.rotation) as Projectile;
				p.transform.parent = gunContainer;
				p.Init(physicalProjectileVelocity, impactDamage,isPlayerWeapon);
				p.SetGravity(masterGravity);
			}
			else
				Debug.LogWarning("Gun has no projectile");
		}
		// OR: Handle ray projectiles
		else if (projectileType == ProjectileType.Raycast) {
			Ray ray = new Ray(spawn.position,spawn.forward);
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, rayProjectileRange, collisionMask)) {
				if (hit.transform.GetComponent<MonoBehaviour>()) {
					// Code for raycast projectile impact goes here. e.g:
					
					//MonoBehaviour hitObject = hit.transform.GetComponent<MonoBehaviour>();	
					//((IDamageable)hitObject).TakeDamage(impactDamage);
				}
			}
		}
	}
	
	// Full magazine to capacity
	private void UpdateAmmoAfterReload() {
		//print (availableAmmo + "   " + magSize);

		ammoRemainingInMag = Mathf.Clamp(availableAmmo,0,magSize);
		availableAmmo-=ammoRemainingInMag;
		ammoMagCounter = ammoRemainingInMag;
		ammoTotalCounter = availableAmmo;
		//availableAmmo -= ammoRemainingInMag;	
	}
	

	private IEnumerator Muzzleflash() {
		foreach (Material m in muzzleFlashMaterials) {
			m.mainTexture = muzzleFlashTextures[Random.Range(0,muzzleFlashTextures.Length)];
			m.color = Color.white;
		}
		yield return null;
		foreach (Material m in muzzleFlashMaterials) {
			m.color = Color.clear;
		}
	}

	// Fire in bursts of burstCount
	private IEnumerator FireBurst() {
		if (Time.time > nextBurstTime) {
			nextBurstTime = Time.time + delayBetweenBursts;
			burstInProgress = true;
			while (projectilesFiredInBurst < burstCount) {
				Shoot ();
				yield return new WaitForSeconds(timeBetweenShots);
			}
			burstInProgress = false;
		}
	}

	// Input methods called by owner to operate gun
	#region Input
	
	// Called by the gun owner to fire the weapon continuously
	public override void LeftMouseHold ()
	{
		if (fireMode == FireMode.Automatic)
			Shoot ();
	}
	
	// Called by the gun owner to fire a single shot/burst
	public override void LeftMouseDown ()
	{
		if (fireMode == FireMode.Burst) {
			projectilesFiredInBurst = 0;
			if (!burstInProgress) {
				StartCoroutine("FireBurst");
			}
		}

		if (fireMode == FireMode.Semi)
			Shoot ();
	}

	// Call from owner class if burst mode should only work while trigger is held down
	public override void LeftMouseUp ()
	{
		if (fireMode == FireMode.Burst) {
			StopCoroutine("FireBurst");
			burstInProgress = false;
		}
	}
	
	// Reload the weapon
	public override void Reload() {
		if (ammoRemainingInMag!= magSize && ammoTotalCounter!=0) {
			base.Reload();
			if (!handling.reloading) {
				PlayAudio(reloadAudio);
				handling.Reload(reloadTime);
				StartCoroutine(FinishReload());
			}
		}
	}

	IEnumerator FinishReload() {
		yield return new WaitForSeconds (reloadTime);
		UpdateAmmoAfterReload();	
	}
	#endregion

	public override void ChangeGravity (FauxGravityAttractor attractor)
	{
		masterGravity = attractor;
	}

	
	
}
