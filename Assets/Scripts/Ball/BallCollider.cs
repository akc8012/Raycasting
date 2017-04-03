using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollider : MonoBehaviour
{
	BallController ballController;

	void Start()
	{
		ballController = GetComponent<BallController>();
	}

	void Update()
	{
		bool isGrounded = false;
		RaycastHit hitMan;
		Ray rayMan = new Ray(transform.position, Vector3.down);

		Vector3 thing = Vector3.zero;

		if (Physics.Raycast(rayMan, out hitMan, 1.2f))
		{
			isGrounded = true;
			thing = Vector3.Cross(hitMan.normal, Vector3.down);
			thing.x *= Mathf.Rad2Deg;
			thing.z *= Mathf.Rad2Deg;
		}

		ballController.SendInfo(isGrounded, Quaternion.Euler(thing));
	}
}
