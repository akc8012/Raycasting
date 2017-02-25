using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCollider
{
	Transform transform;
	PlayerController.OnFloor onFloor;

	Vector3[] origins;
	Vector3[] directions;
	int[,] originsForDirs;

	float skinLength = 0.2f;
	float rayLength = 0.2f;
	float downRayVelMod = 0.02f;

	public void Init(Transform transform, PlayerController.OnFloor onFloor)
	{
		this.transform = transform;
		this.onFloor = onFloor;

		directions = new Vector3[] { Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
		originsForDirs = new int[,] { { 0, 1, 2, 3 }, { 2, 3, -1, -1 }, { 0, 1, -1, -1 },
									  { 0, 2, -1, -1 }, { 1, 3, -1, -1 } };
	}

	public void CustomUpdate(float playerDownVel)
	{
		origins = new Vector3[] { GetBottomLeft, GetBottomRight, GetTopLeft, GetTopRight };

		for (int i = 0; i < directions.Length; i++)
		{
			for (int j = 0; j < origins.Length; j++)
			{
				if (originsForDirs[i, j] == -1) continue;

				Vector3 hitPos;
				float mod = DownRayMod(playerDownVel, i);
				if (ShootRay(origins[originsForDirs[i,j]], directions[i], out hitPos, mod))
				{
					int axis = GetAxisOfDirection(directions[i]);
					float pos = hitPos[axis] - (directions[i][axis] * GetExtents[axis]);

					SetPos(axis, pos);
				}
			}
		}
	}

	bool ShootRay(Vector3 origin, Vector3 direction, out Vector3 hitPos, float newLength)
	{
		origin -= direction * skinLength;     // when we're on the floor, make sure ray is high enough to still touch the floor
		float length = newLength == -1 ? rayLength : newLength;

		RaycastHit hitMan;
		Ray rayMan = new Ray(origin, direction);
		Debug.DrawLine(rayMan.origin, rayMan.origin + (rayMan.direction*length), Color.white);
		if (Physics.Raycast(rayMan, out hitMan, length))
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

	int GetAxisOfDirection(Vector3 direction)
	{
		for (int i = 0; i < 3; i++)
		{
			if (direction[i] != 0)
				return i;
		}
		return -1;
	}

	float DownRayMod(float playerDownVel, int i)
	{
		if (i == 0)
		{
			playerDownVel = -playerDownVel * downRayVelMod;
			return Mathf.Clamp(playerDownVel, rayLength, float.MaxValue);
		}

		return -1;
	}

	public Vector3 GetExtents { get { return transform.lossyScale / 2; } }

	public Vector3 GetMax { get { return transform.position + GetExtents; } }   // top right
	public Vector3 GetMin { get { return transform.position - GetExtents; } }   // bottom left

	public Vector3 GetBottomLeft { get { return GetMin; } }
	public Vector3 GetTopRight { get { return new Vector3(transform.position.x + GetExtents.x, transform.position.y - GetExtents.y, transform.position.z + GetExtents.z); } }
	public Vector3 GetTopLeft { get { return new Vector3(transform.position.x - GetExtents.x, transform.position.y - GetExtents.y, transform.position.z + GetExtents.z); } }
	public Vector3 GetBottomRight { get { return new Vector3(transform.position.x + GetExtents.x, transform.position.y - GetExtents.y, transform.position.z - GetExtents.z); } }

}
