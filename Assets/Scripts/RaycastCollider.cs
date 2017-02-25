using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCollider
{
	Transform transform;
	PlayerController.OnFloor onFloor;

	public void Init(Transform transform, PlayerController.OnFloor onFloor)
	{
		this.transform = transform;
		this.onFloor = onFloor;
	}

	public void CustomUpdate()
	{
		Vector3[] origins = new Vector3[] { GetBottomLeft, GetBottomRight, GetTopLeft, GetTopRight };
		Vector3[] directions = new Vector3[] { Vector3.down, Vector3.forward };
		//int[] axisies = new int[] { 1, 2, 2, 0, 0 };
		for (int i = 0; i < origins.Length; i++)
		{
			for (int j = 0; j < directions.Length; j++)
			{
				if (j == 1 && (i == 0 || i == 1))
					continue;

				Vector3 hitPos;
				if (ShootRay(origins[i], directions[j], out hitPos))
				{
					int axis;
					float pos;

					if (directions[j] == Vector3.down)
						axis = 1;
					else
						axis = 2;

					if (directions[j] == Vector3.down)
						pos = hitPos.y + GetExtents.y;
					else
						pos = hitPos.z - GetExtents.z;

					SetPos(axis, pos);
					//break;
				}
			}
		}
	}

	bool ShootRay(Vector3 origin, Vector3 direction, out Vector3 hitPos)
	{
		if (direction == Vector3.down)
			origin.y += 0.2f;       // "skin width" (when we're on the floor, make sure ray is high enough to still touch the floor)
		else
			origin.z -= 0.2f;

		RaycastHit hitMan;
		Ray rayMan = new Ray(origin, direction);
		Debug.DrawLine(rayMan.origin, rayMan.origin + (rayMan.direction*0.3f), Color.white);
		if (Physics.Raycast(rayMan, out hitMan, 0.2f))
		{
			hitPos = hitMan.point;
			return true;
		}

		hitPos = Vector3.zero;
		return false;
	}

	void SetPos(int axis, float pos)
	{
		if (axis == 1) onFloor();

		Vector3 newPos = transform.position;
		newPos[axis] = pos;
		transform.position = newPos;
	}

	public Vector3 GetExtents { get { return transform.lossyScale / 2; } }

	public Vector3 GetMax { get { return transform.position + GetExtents; } }   // top right
	public Vector3 GetMin { get { return transform.position - GetExtents; } }   // bottom left

	public Vector3 GetBottomLeft { get { return GetMin; } }
	public Vector3 GetTopRight { get { return new Vector3(transform.position.x + GetExtents.x, transform.position.y - GetExtents.y, transform.position.z + GetExtents.z); } }
	public Vector3 GetTopLeft { get { return new Vector3(transform.position.x - GetExtents.x, transform.position.y - GetExtents.y, transform.position.z + GetExtents.z); } }
	public Vector3 GetBottomRight { get { return new Vector3(transform.position.x + GetExtents.x, transform.position.y - GetExtents.y, transform.position.z - GetExtents.z); } }

}
