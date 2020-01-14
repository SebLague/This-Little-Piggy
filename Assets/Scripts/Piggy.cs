using UnityEngine;
using System.Collections;

public class Piggy : Lifeform {

	private Player player;
	private float targetRot;
	private float targetSpeed;

	public float moveSpeed = 3;
	public float runSpeed;
	public float turnSpeed = 2;
	float walkSpeed;
	float currentSpeed;
	float currentRot;

	Vector3 moveAmount;
	public GameObject ragdoll;

	private bool hurt;
	private float hurtEndTime;

	public override void Awake ()
	{
		base.Awake ();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		walkSpeed = moveSpeed;
		StartCoroutine("AI");
		currentRot = transform.localEulerAngles.y;
		targetRot = currentRot;
	}

	IEnumerator AI() {
		float nextMoveTime = 0;
		float nextStopTime = Time.time;
		float nextTurnTime = Time.time;

		bool stopped = true;

		while (true){

			if (Time.time > nextMoveTime && stopped) {
				targetSpeed = 1;
				nextStopTime = Time.time + Random.Range(6f,15f);
				stopped = false;
			}
			if (!hurt) {
				if (Time.time > nextStopTime && !stopped) {
					targetSpeed = 0;
					nextMoveTime = Time.time + Random.Range(1,3.5f);
					stopped = true;
				}
			}

			if (Time.time > nextTurnTime && !stopped) {
				targetRot += Random.Range(-15,15);
				nextTurnTime = Time.time + Random.Range(5f,12);
			}

			if (hurt) {
				if (Time.time > hurtEndTime) {
					hurt = false;
					moveSpeed = walkSpeed;
				}
			}

			yield return new WaitForSeconds(1);

		}
	}

	void Update() {
		//currentSpeed = targetSpeed;
		currentSpeed = Mathf.MoveTowards(currentSpeed,targetSpeed,5);
		float speedPercent = currentSpeed;
		anim.SetFloat("Speed",speedPercent);

		moveAmount = Vector3.forward * currentSpeed * moveSpeed;
		//currentRot = Mathf.MoveTowardsAngle(currentRot,targetR
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.up * targetRot), Time.deltaTime * turnSpeed *speedPercent );
		//print (transform.localEulerAngles.y + "   :   " + targetRot);

	}

	public override void TakeDamage (float dmg,bool p)
	{
		base.TakeDamage (dmg,p);
		moveSpeed = runSpeed;
		targetSpeed = moveSpeed;
		hurt = true;
		hurtEndTime = Time.time + 15;
	}

	public override void Die ()
	{
		if (!dead) {
			base.Die ();
			anim.enabled = false;
			Instantiate(ragdoll,transform.position,transform.rotation);
			player.PigKilled();
			Destroy(gameObject);
		}

	}

	void FixedUpdate() {
		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveAmount) * Time.deltaTime);

	}

	public override void OnTriggerEnter (Collider c)
	{

	}
}
