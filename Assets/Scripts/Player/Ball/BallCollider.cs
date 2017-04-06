using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollider : MonoBehaviour
{
	[SerializeField] float maxClimbAngle = 35;

	void Start()
	{
		
	}

	public void CustomUpdate(out bool isGrounded, out Quaternion floorRot)
	{
		RaycastHit hitMan;
		Ray rayMan = new Ray(transform.position, Vector3.down);

		Vector3 rot = Vector3.zero;

		if (Physics.Raycast(rayMan, out hitMan, 1.2f))
		{
			isGrounded = true;
			rot = Vector3.Cross(hitMan.normal, Vector3.down);
			rot.x *= Mathf.Rad2Deg;
			rot.z *= Mathf.Rad2Deg;

			if (Mathf.Abs(rot.x) >= maxClimbAngle || Mathf.Abs(rot.z) >= maxClimbAngle)
				isGrounded = false;
			
		} else isGrounded = false;

		floorRot = Quaternion.Euler(rot);
	}
}
