using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanController : MonoBehaviour
{
	Text displayText;

	HumanCollider raycastCol;
	Transform cam;

	[SerializeField] Transform rotateMesh;
	[SerializeField] Animator animator;

	[SerializeField] float maxSpeed = 6;       // what to increment velocity by
	[SerializeField] float gravity = 35;
	[SerializeField] float jumpSpeed = 14;
	[SerializeField] float acceleration = 12.5f;
	[SerializeField] float deceleration = 46.8f;
	[SerializeField] bool doAnimations = true;

	Vector3 lastVel;
	float jumpyVel;
	float lastSpeed = 0;
	float speedJumpedAt;
	float currJumpSpeed;

	const float rotSmooth = 20;          // smoothing on the lerp to rotate towards stick direction
	const float rotSmoothSlow = 5;
	const float maxFallSpeed = 30;
	const float jumpDetraction = 0.25f;
	const float fallDownFast = 0.90f;

	bool isGrounded = false;
	public bool IsGrounded { get { return isGrounded; } }
	public bool IsRising { get { return lastVel.y > 0; } }

	public Vector3 GetVel { get { return lastVel; } }

	public delegate void OnFloor();

	void Awake()
	{
		cam = GameObject.FindWithTag("MainCamera").transform;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;

		currJumpSpeed = jumpSpeed;

		raycastCol = GetComponent<HumanCollider>();
		raycastCol.Init(onFloor);

		if (GameObject.Find("Text"))
			displayText = GameObject.Find("Text").GetComponent<Text>();
	}

	void Update()
	{
		if (Time.deltaTime > 0.1f) return;

		float speed = 0;
		Vector3 moveDir = GetMoveDirection(ref speed);
		RotateMesh(moveDir);

		if (speed > 0.19f)		// greater than deadzone
			SpeedUp(ref speed);
		else
			SlowDown(ref speed);

		if (doAnimations) animator.SetFloat("Speed", speed);
		
		Vector3 vel = rotateMesh.forward * speed;
		vel.y = lastVel.y + jumpyVel;

		HandleJumpInput(Input.GetButtonDown("Jump"), Input.GetButton("Jump"), speed, ref vel);

		Move(ref vel);

		isGrounded = false;
		lastSpeed = speed;
		lastVel = vel;
		DebugStuff(speed);
	}

	void LateUpdate()
	{
		raycastCol.CustomUpdate(lastVel.y);
	}

	Vector3 GetMoveDirection(ref float speed)
	{
		Vector3 stickDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		speed = Mathf.Clamp(Vector3.Magnitude(stickDir), 0, 1);

		Vector3 cameraDir = cam.forward; cameraDir.y = 0.0f;
		Vector3 moveDir = Quaternion.FromToRotation(Vector3.forward, cameraDir) * stickDir;     // referential shift

		// fixes bug when the camera forward is exactly -forward (opposite to Vector3.forward) by flipping the x around
		if (Vector3.Dot(Vector3.forward, cameraDir.normalized) == -1)
			moveDir = new Vector3(-moveDir.x, moveDir.y, moveDir.z);

		return moveDir;
	}

	void RotateMesh(Vector3 moveDir)
	{
		float angle = Vector3.Angle(moveDir, rotateMesh.forward);

		//if (angle > 15 && angle != 90)
		//	return;

		if (angle > 135)        // if we're a very big angle change, we'll want to snap right to it, instead of lerping
			rotateMesh.forward = moveDir;
		else
		{
			Vector3 targetRotation = Vector3.Lerp(rotateMesh.forward, moveDir,
				Time.deltaTime * (IsGrounded ? rotSmooth : rotSmoothSlow));
			if (targetRotation != Vector3.zero)
				rotateMesh.rotation = Quaternion.LookRotation(targetRotation);
		}
	}

	void SpeedUp(ref float speed)
	{
		float speedClamp = speed;
		float airClamp = (IsGrounded ? 1 : (speedJumpedAt / maxSpeed) + 0.2f);
		speed = lastSpeed + (acceleration * Time.deltaTime);
		speed = Mathf.Clamp(speed, 0, maxSpeed * speedClamp * airClamp);
	}

	void SlowDown(ref float speed)
	{
		speed = lastSpeed - (deceleration * Time.deltaTime);
		speed = Mathf.Clamp(speed, 0, maxSpeed);
	}

	void HandleJumpInput(bool jumpButtonDown, bool jumpHeld, float speed, ref Vector3 vel)
	{
		if (jumpButtonDown && IsGrounded)
		{
			if (doAnimations) animator.SetTrigger("Jump");
			speedJumpedAt = speed;
			StartCoroutine("Jump");
			return;
		}

		if ((!jumpHeld && !isGrounded) || !IsRising)
		{
			if (IsRising)      // set vel to fall down faster
				vel.y *= fallDownFast;

			StopCoroutine("Jump");
			currJumpSpeed = jumpSpeed;
			jumpyVel = 0;
		}
	}

	IEnumerator Jump()
	{
		float valueBasedOnRunningJumpSpeed = Mathf.Clamp((speedJumpedAt / maxSpeed) + 0.4f, 0.9f, 1.14f);
		while (true)
		{
			jumpyVel = jumpyVel + (currJumpSpeed * valueBasedOnRunningJumpSpeed);
			currJumpSpeed *= jumpDetraction;
			yield return null;
		}
	}

	void Move(ref Vector3 vel)
	{
		vel.y -= gravity * Time.deltaTime;
		vel.y = Mathf.Clamp(vel.y, -maxFallSpeed, float.MaxValue);
		transform.position += vel * Time.deltaTime;
	}

	void onFloor()
	{
		lastVel.y = 0;
		isGrounded = true;
	}

	void DebugStuff(float speed)
	{
		if (displayText)
			displayText.text = speed + "";
	}
}
