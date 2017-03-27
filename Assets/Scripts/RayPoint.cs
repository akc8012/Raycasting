using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPoint : MonoBehaviour
{
	[SerializeField] Direction direction;
	enum Direction { Down, Forward, Back, Left, Right };
	public Vector3 GetDirection
	{
		get
		{
			switch (direction)
			{
				case Direction.Down:
				return Vector3.down;

				case Direction.Forward:
				return Vector3.forward;

				case Direction.Back:
				return Vector3.back;

				case Direction.Left:
				return Vector3.left;

				case Direction.Right:
				return Vector3.right;

				default:
				return Vector3.zero;
			}
		}
	}


	public Vector3 GetPosition { get { return transform.position; } }
}
