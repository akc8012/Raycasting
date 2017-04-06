using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCollider : MonoBehaviour
{
	[SerializeField] Vector3 size = Vector3.one;
	[SerializeField] GameObject rayPointsRoot;
	RayPoint[] rayPoints;

	GameObject drawBox;
	HumanController.OnFloor onFloor;

	const float skinLength = 12.5f;
	const float rayLength = 12.5f;
	const float downRayVelMod = 1.25f;
	const float backRayMod = 18.75f;
	const float dotAllowance = -0.5f;	// lower is less lenient

	public void Init(HumanController.OnFloor onFloor)
	{
		this.onFloor = onFloor;

		rayPoints = rayPointsRoot.GetComponentsInChildren<RayPoint>();
		drawBox = transform.Find("Collider Draw Box").gameObject;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, size);
	}

	public void CustomUpdate(float playerDownVel, Vector3 playerBack)
	{
		for (int i = 0; i < rayPoints.Length; i++)
		{
			Vector3 hitPos;

			float length = GetRayLength(i, playerDownVel, rayPoints[i].GetDirection, playerBack);

			if (ShootRay(rayPoints[i].GetPosition, rayPoints[i].GetDirection, length, out hitPos))
			{
				int axis = GetAxisOfDirection(rayPoints[i].GetDirection);
				float pos = hitPos[axis] - (rayPoints[i].GetDirection[axis] * GetExtents[axis]);

				SetPos(axis, pos);
			}
		}

		if (Input.GetKeyDown(KeyCode.C))
			drawBox.SetActive(!drawBox.activeSelf);
	}

	bool ShootRay(Vector3 origin, Vector3 direction, float length, out Vector3 hitPos)
	{
		origin -= direction * skinLength*Time.deltaTime;     // when we're on the floor, make sure ray is high enough to still touch the floor

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

	float GetRayLength(int rayNdx, float playerDownVel, Vector3 rayDir, Vector3 playerBack)
	{
		if (rayPoints[rayNdx].GetRealDirection == RayPoint.Direction.Down)
		{
			float backRay = 0;
			if (IsBackRay(rayNdx, playerBack))
				backRay = backRayMod * Time.deltaTime;

			playerDownVel = -playerDownVel * downRayVelMod * Time.deltaTime;
			float gravityRay = Mathf.Clamp(playerDownVel, rayLength*Time.deltaTime, float.MaxValue);

			return Mathf.Max(gravityRay, backRay);
		}

		return rayLength*Time.deltaTime;
	}

	bool IsBackRay(int rayNdx, Vector3 playerBack)
	{
		Vector3 backPoint = transform.position + playerBack;	// extend or retract this based on the "radius" of collider
		float minDist = float.MaxValue;
		int targetI = -1;

		for (int i = 0; i < rayPoints.Length; i++)
		{
			if (rayPoints[i].GetRealDirection == RayPoint.Direction.Down)
			{
				float dist = Vector3.Distance(rayPoints[i].GetPosition, backPoint);
				if (dist < minDist)
				{
					minDist = dist;
					targetI = i;
				}
			}
		}

		return (targetI == rayNdx);
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
