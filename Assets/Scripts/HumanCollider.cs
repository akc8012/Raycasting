using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCollider : MonoBehaviour
{
	[SerializeField] Vector3 size = Vector3.one;
	[SerializeField] GameObject rayPointsRoot;
	RayPoint[] rayPoints;

	HumanController.OnFloor onFloor;

	float skinLength = 0.2f;
	float rayLength = 0.2f;
	float downRayVelMod = 1.25f;
	float dotAllowance = -0.5f;	// lower is less lenient

	public void Init(HumanController.OnFloor onFloor)
	{
		this.onFloor = onFloor;

		rayPoints = rayPointsRoot.GetComponentsInChildren<RayPoint>();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, size);
	}

	public void CustomUpdate(float playerDownVel)
	{
		for (int i = 0; i < rayPoints.Length; i++)
		{
			Vector3 hitPos;
			float mod = DownRayMod(playerDownVel, i);
			if (ShootRay(rayPoints[i].GetPosition, rayPoints[i].GetDirection, out hitPos, mod))
			{
				int axis = GetAxisOfDirection(rayPoints[i].GetDirection);
				float pos = hitPos[axis] - (rayPoints[i].GetDirection[axis] * GetExtents[axis]);

				SetPos(axis, pos);
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
			float dot = Vector3.Dot(rayMan.direction, hitMan.normal);
			if (dot > dotAllowance)
			{
				hitPos = Vector3.zero;
				return false;
			}

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
			playerDownVel = -playerDownVel * downRayVelMod * Time.deltaTime;
			return Mathf.Clamp(playerDownVel, rayLength, float.MaxValue);
		}

		return -1;
	}

	public Vector3 GetExtents { get { return size / 2; } }

	public Vector3 GetMax { get { return transform.position + GetExtents; } }   // top right
	public Vector3 GetMin { get { return transform.position - GetExtents; } }   // bottom left

	public Vector3 GetBottomCenter { get { Vector3 pos = transform.position; pos.y -= GetExtents.y; return pos; } }
	public Vector3 GetBottomLeft { get { return GetMin; } }
	public Vector3 GetTopRight { get { return new Vector3(transform.position.x + GetExtents.x, transform.position.y - GetExtents.y, transform.position.z + GetExtents.z); } }
	public Vector3 GetTopLeft { get { return new Vector3(transform.position.x - GetExtents.x, transform.position.y - GetExtents.y, transform.position.z + GetExtents.z); } }
	public Vector3 GetBottomRight { get { return new Vector3(transform.position.x + GetExtents.x, transform.position.y - GetExtents.y, transform.position.z - GetExtents.z); } }

}
