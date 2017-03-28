using UnityEngine;
using System.Collections;

// Placement:  Place on 'GroundCheck' gameobject

// Purpose:  Tells the player if it is grounded or not, and modifies movement
// 				based on slope angle.

public class GroundCheck_Grounded : MonoBehaviour {

	GameObject 
		player;

	void Start () 
	{
		
	}

	void Update () 
	{
		player = GameObject.FindWithTag ("Player");
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Ramp") {
			player.GetComponent<BallController> ().IsJumping (false);
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Ramp") {
			player.GetComponent<BallController> ().IsGrounded (true);
		}

		if(col.gameObject.tag == "Ramp"){
			if (col.GetComponent<Terrain_ManageableTerrain>().ManageableTerrain == false) {
				player.GetComponent<BallController> ().ToggleControls (false);
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		player.GetComponent<BallController> ().ToggleControls (true);

		if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Ramp") {
			player.GetComponent<BallController> ().IsGrounded (false);
		}
	}
}
