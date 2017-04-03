using UnityEngine;
using System.Collections;

// Placement:  Place on 'GroundCheck' gameobject

// Purpose:  Tells the player if it is grounded or not, and modifies movement
// 				based on slope angle.

public class GroundCheck_Grounded : MonoBehaviour
{
	BallController ballController;

	void Start () 
	{
		ballController = GetComponent<BallController>();
	}

	void OnCollisionEnter(Collision col){
		//if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Ramp") {
		//	ballController.IsJumping (false);
		//}
	}

	void OnCollisionStay(Collision col)
	{
		//if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Ramp") {
			ballController.IsGrounded (true);
		//}

		//if(col.gameObject.tag == "Ramp"){
		//	if (col.gameObject.GetComponent<Terrain_ManageableTerrain>().ManageableTerrain == false) {
		//		ballController.ToggleControls (false);
		//	}
		//}
	}

	void OnCollisionExit(Collision col)
	{
		//ballController.ToggleControls (true);

		//if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Ramp") {
			ballController.IsGrounded (false);
		//}
	}
}
