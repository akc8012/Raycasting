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
		RaycastHit hitMan;
		Ray rayMan = new Ray(bottom.position, Vector3.down);
		if (Physics.Raycast(rayMan, out hitMan, 20))
		{
			Vector3 distance = hitMan.point - bottom.position;
			transform.position += distance;
		}
	}
}
