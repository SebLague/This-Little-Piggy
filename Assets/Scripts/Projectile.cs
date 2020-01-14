using UnityEngine;
using System.Collections;

public class Projectile : GravityObject {

	private const float rayBuffer = .1f;

	public string[] hitTags; /// The hit object tags that correlate to arrays of hit effects and audio below
	public GameObject[] hitEffects;
	public AudioClip[] hitAudio;
	public LayerMask collisionMask;

	private TrailRenderer trail;
	private float trailTimeOld;
	public bool explode;
	public GameObject explosion;
	public float explodeRadius;
	public LayerMask explosionHurtMask;
	bool playerProjectile;
	
	// System
	private float velocity;
	private float impactDamage;
	
	public void Init(float velocityP, float impactDamageP, bool isPlayer) {
		playerProjectile = isPlayer;
		if (GetComponent<TrailRenderer>())
			trail = GetComponent<TrailRenderer>();
		velocity = velocityP;
		impactDamage = impactDamageP;
		if (explode) {
			Invoke("Explode",4);
		}
		else 
			Destroy(gameObject,5);
		//collisionMask = collisionMaskP;
	}
	
	void Update () {

		float forwardMoveDst = velocity * Time.deltaTime;
		
		// Check collisions
		Ray ray = new Ray(transform.position,transform.forward);
		RaycastHit hit;
		//Debug.DrawRay(ray.origin,ray.direction * 5,Color.red,3);
		if (Physics.Raycast(ray, out hit, forwardMoveDst + rayBuffer, collisionMask)) {
			// Impact code goes here
			if (explode) {
				Explode ();
			}
			else {
				IDamageable hitObject = hit.transform.GetComponent(typeof(IDamageable)) as IDamageable;
				if (hitObject != null) {
					hitObject.TakeDamage(impactDamage,playerProjectile);
				}
			}

			Destroy(gameObject);
		}
		// Move
		Vector3 move = Vector3.forward * forwardMoveDst;
		transform.Translate(move);

	}

	public override void Teleport() {
		base.Teleport();
		if (trail) {
			trailTimeOld = trail.time;
			trail.time = -1;
			Invoke("ResetTrail",.05f);
		}
	}

	public override void UpdateGravity ()
	{
		base.UpdateGravity ();

	}

	void ResetTrail() {
		print (trail.time);
		trail.time = trailTimeOld;
		print (trail.time);
	}
	
	void Explode() {
		foreach (Collider c in Physics.OverlapSphere(transform.position,explodeRadius,explosionHurtMask)) {
			IDamageable hitObject = c.gameObject.GetComponent(typeof(IDamageable)) as IDamageable;
			if (hitObject != null) {
				float dstPercent = Vector3.Distance(transform.position,c.transform.position)/explodeRadius;
				hitObject.TakeDamage(Mathf.Lerp(impactDamage,0,dstPercent),playerProjectile);
			}
		}
		if (explosion)
			Instantiate(explosion,transform.position,transform.rotation);
		Destroy(gameObject);
	}
	
}
