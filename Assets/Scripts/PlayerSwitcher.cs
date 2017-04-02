using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
	enum PlayerState { Human, Ball };
	PlayerState state;
	float lastAxis;

	Rigidbody rb;
	[SerializeField] GameObject humanMesh;
	[SerializeField] GameObject ballMesh;
	HumanController humanController;
	BallController ballController;
	bool enableMeshSwitch = true;

	void Awake ()
	{
		state = PlayerState.Human;
		rb = GetComponent<Rigidbody>();
		humanController = GetComponent<HumanController>();
		ballController = GetComponent<BallController>();

		SwitchState(state);
	}

	public void DisableMeshSwitch()
	{
		enableMeshSwitch = false;
	}
	
	void Update ()
	{
		float currentAxis = Input.GetButton("BallToggle") == true ? -1 : 0;
		if (currentAxis == 0) currentAxis = Input.GetAxisRaw("BallToggle");

		if (currentAxis != lastAxis && (CloseEnough(currentAxis, 0.0f) || CloseEnough(currentAxis, -1.0f)))
		{
			if (CloseEnough(currentAxis, 0.0f))
				state = PlayerState.Human;
			else
				state = PlayerState.Ball;

			SwitchState(state);
		}

		lastAxis = SnapToANum(currentAxis, 0.0f, -1.0f);
	}

	bool CloseEnough(float num, float target)
	{
		return (Mathf.Abs(num - target) < 0.3f);
	}

	float SnapToANum(float num, float targetA, float targetB)
	{
		float a = Mathf.Abs(num - targetA);
		float b = Mathf.Abs(num - targetB);

		if (a < b) return targetA;
		else return targetB;
	}

	void SwitchState(PlayerState newState)
	{
		switch (newState)
		{
			case PlayerState.Human:
				ballMesh.SetActive(false);
				humanMesh.SetActive(true);

				ballController.enabled = false;

				rb.isKinematic = true;
				rb.useGravity = false;
				//rb.detectCollisions = false;

				transform.rotation = Quaternion.identity;
				transform.position += Vector3.up * 0.3f;
				humanController.enabled = true;
			break;

			case PlayerState.Ball:
				if (enableMeshSwitch)
				{
					humanMesh.SetActive(false);
					ballMesh.SetActive(true);
				} else humanMesh.SetActive(true);

				humanController.enabled = false;

				rb.isKinematic = false;
				rb.useGravity = true;
				//rb.detectCollisions = true;

				ballController.enabled = true;
			break;
		}
	}
}
