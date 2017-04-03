using UnityEngine;
using System.Collections;

// Placement:  Place on 'Player' gameobject

// Function:  Contains all movement and input information



// grounded -- slightly weaken controls for air movement
// disable controls -- when climbing up a slope that is tooooo high 

public class BallController : MonoBehaviour
{
	BallCollider ballCollider;
	Rigidbody rb;
	Transform cam;

	Quaternion floorRot;

	[SerializeField] int speed = 10;
	[SerializeField] int maxVelocity = 20;
	[SerializeField] float jumpHeight = 10;
	[SerializeField] float extraGravity = 3;

	[SerializeField][Range(0.90f,0.99f)]
	float playerSlowdownSpeed = 0.95f;

	bool grounded = false;

	void Start () 
	{
		ballCollider = GetComponent<BallCollider>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main.transform;
	}

	// called before rendering a frame
	void Update () 
	{
	
	}

	// called before any physics calculations (put physics here)
	void FixedUpdate()
	{
		ballCollider.CustomUpdate(out grounded, out floorRot);

		Vector3 input = Vector3.zero;

		if (grounded)
			input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		Grounded ();
		Vector3 movement = GetMovement(input) * speed;

		if (Input.GetButtonDown("Jump") && grounded)
		{
			print("jump");
			rb.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
		}

		if (rb.velocity.magnitude < maxVelocity)
			rb.AddForce(movement);

		MovementLimits(movement);
		rb.AddForce(Vector3.down * extraGravity, ForceMode.VelocityChange); // add extra gravity
	}

	Vector3 GetMovement(Vector3 input)
	{
		input.Normalize();
		Vector3 cameraDir = cam.forward; cameraDir.y = 0.0f;
		Vector3 moveDir = Quaternion.FromToRotation(Vector3.forward, cameraDir) * input;     // referential shift
		moveDir = floorRot * moveDir;

		// fixes bug when the camera forward is exactly -forward (opposite to Vector3.forward) by flipping the x around
		if (Vector3.Dot(Vector3.forward, cameraDir.normalized) == -1)
			moveDir = new Vector3(-moveDir.x, moveDir.y, moveDir.z);


		Debug.DrawLine(rb.position, rb.position + (moveDir * 3), Color.blue);

		return moveDir;
	}

	// This prevents the player from flying off of ledges and ramps
	// Also limits air control to the degree specified
	void Grounded()
	{
		if (!grounded)
			rb.mass = 100;
		else
			rb.mass = 1;
	}

	// When no keys are pressed, slow down the player faster allowing
	// for faster direction shifts
	void MovementLimits(Vector3 movement)
	{
		if (movement == Vector3.zero)
		{
			rb.mass = 100;

			if (grounded)
				rb.velocity *= playerSlowdownSpeed;
		}
		else
		{
			rb.mass = 1;
		}
	}
}
