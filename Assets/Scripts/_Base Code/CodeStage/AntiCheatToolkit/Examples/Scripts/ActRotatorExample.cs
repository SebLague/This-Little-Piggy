using UnityEngine;

// dummy code, just to add some rotation to the cube from example scene
public class ActRotatorExample : MonoBehaviour
{
	[Range(1f, 100f)]
	public float speed = 5f;
	private void Update()
	{
		transform.Rotate(speed * Time.deltaTime, speed * Time.deltaTime, speed * Time.deltaTime);
	}
}
