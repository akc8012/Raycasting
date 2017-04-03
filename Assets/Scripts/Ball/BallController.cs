﻿using UnityEngine;
using System.Collections;

// Placement:  Place on 'Player' gameobject

// Function:  Contains all movement and input information

public class BallController : MonoBehaviour
{
	Rigidbody rb;
	Transform cam;

	[SerializeField]
	int 
		speed = 10,
		maxVelocity = 20,
		jumpPower = 5,
		jumpPowerSecond = 5;

	[SerializeField][Range(0.90f,0.99f)]
	float
		playerSlowdownSpeed = 0.95f;

	[SerializeField]
	bool 
		grounded = false,
		controlsToggle = true,
		jumping = false,
		jump = false,
		secondJump = false;

	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		cam = Camera.main.transform;
	}

	// called before rendering a frame
	void Update () 
	{
	
	}

	public void ToggleControls(bool check)
	{
		controlsToggle = check;
	}

	public void IsGrounded(bool check)
	{
		grounded = check;
	}

	public void IsJumping(bool check)
	{
		jumping = check;
	}

	// called before any physics calculations (put physics here)
	void FixedUpdate()
	{
		Vector3 input = Vector3.zero;

		if (controlsToggle)
		{
			input.x = Input.GetAxisRaw ("Horizontal");
			input.z = Input.GetAxisRaw ("Vertical");
			jump = false;	//Input.GetButtonDown ("Jump");
		}

		Grounded ();

		Vector3 movement = GetMovement(input) * speed;

		if (jumping)
			movement *= 0.6f;

		if (rb.velocity.magnitude < maxVelocity)
			rb.AddForce(movement);

		if (secondJump && jumping && jump)
		{
			Vector3 temp = rb.velocity;
			temp.y = 0;
			rb.velocity = temp;
			rb.AddForce (0, jumpPowerSecond, 0);
			secondJump = false;
		}

		if (grounded && jump)
		{
			rb.AddForce (0, jumpPower, 0);
			jumping = true;
			secondJump = true;
			rb.mass = 2;
		}

		MovementLimits(movement);
	}

	Vector3 GetMovement(Vector3 input)
	{
		Vector3 cameraDir = cam.forward; cameraDir.y = 0.0f;
		Vector3 moveDir = Quaternion.FromToRotation(Vector3.forward, cameraDir) * input;     // referential shift

		// fixes bug when the camera forward is exactly -forward (opposite to Vector3.forward) by flipping the x around
		if (Vector3.Dot(Vector3.forward, cameraDir.normalized) == -1)
			moveDir = new Vector3(-moveDir.x, moveDir.y, moveDir.z);

		return moveDir;
	}

	// This prevents the player from flying off of ledges and ramps
	// Also limits air control to the degree specified
	void Grounded()
	{
		if (!grounded && !jumping)
			rb.mass = 100;
		else if(!jumping)
			rb.mass = 1;
	}

	// When no keys are pressed, slow down the player faster allowing
	// for faster direction shifts
	void MovementLimits(Vector3 movement)
	{
		if (movement == Vector3.zero)
		{
			rb.mass = 100;

			if(grounded && controlsToggle)
				rb.velocity *= playerSlowdownSpeed;
		}
		else if (!jumping)
		{
			rb.mass = 1;
		}
	}
}
