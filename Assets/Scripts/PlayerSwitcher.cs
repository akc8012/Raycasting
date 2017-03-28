using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
	enum PlayerState { Human, Ball };
	PlayerState state;

	Rigidbody rb;
	[SerializeField] GameObject humanMesh;
	[SerializeField] GameObject ballMesh;
	HumanController humanController;
	BallController ballController;

	void Awake ()
	{
		state = PlayerState.Human;
		rb = GetComponent<Rigidbody>();
		humanController = GetComponent<HumanController>();
		ballController = GetComponent<BallController>();

		SwitchState(state);
	}
	
	void Update ()
	{
		if (Input.GetMouseButtonDown(1))
		{
			if (state == PlayerState.Human)
				state = PlayerState.Ball;
			else
				state = PlayerState.Human;

			SwitchState(state);
		}
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
				rb.detectCollisions = false;

				transform.rotation = Quaternion.identity;
				humanController.enabled = true;
			break;

			case PlayerState.Ball:
				humanMesh.SetActive(false);
				ballMesh.SetActive(true);

				humanController.enabled = false;

				rb.isKinematic = false;
				rb.useGravity = true;
				rb.detectCollisions = true;

				ballController.enabled = true;
			break;
		}
	}
}
