using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticker : MonoBehaviour
{
	[SerializeField] Transform bottom;

	void Start()
	{
		
	}
	
	void Update()
	{
		FrontRay();
		DownRay();
	}

	bool FrontRay()
	{
		RaycastHit hitMan;
		Vector3 startPoint = transform.position + new Vector3(0, 0, 0.4f);
		Ray rayMan = new Ray(startPoint, Vector3.forward);

		Debug.DrawRay(rayMan.origin, rayMan.direction);

		if (Physics.Raycast(rayMan, out hitMan, 1))
		{
			float dot = Vector3.Dot(rayMan.direction, hitMan.normal);
			print(dot);
			if (dot > -0.5f) return false;

			Vector3 distance = hitMan.point - startPoint;
			transform.position += distance;
			return true;
		}

		return false;
	}

	bool DownRay()
	{
		RaycastHit hitMan;
		Ray rayMan = new Ray(bottom.position + new Vector3(0, 0.2f, 0), Vector3.down);

		Debug.DrawRay(rayMan.origin, rayMan.direction);

		if (Physics.Raycast(rayMan, out hitMan, 1))
		{
			float dot = Vector3.Dot(rayMan.direction, hitMan.normal);
			print(dot);
			if (dot > -0.5f) return false;

			Vector3 distance = hitMan.point - bottom.position;
			transform.position += distance;
			return true;
		}

		return false;
	}
}
