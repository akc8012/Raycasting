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
		if (Physics.Raycast(rayMan, out hitMan, 1.2f))
		{
			isGrounded = true;
		}

		ballController.IsGrounded(isGrounded);
	}
}
