using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarbleManager : MonoBehaviour
{
	[SerializeField] Text marbleText;

	GameObject[] marbles;
	int collected = 0;

	void Start()
	{
		marbles = GameObject.FindGameObjectsWithTag("Marble");
		marbleText.text = "0 / " + marbles.Length;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha2))
			Respawn();
	}

	public void GetMarble()
	{
		collected++;
		marbleText.text = collected + " / " + marbles.Length;

		if (collected >= marbles.Length)
			GameObject.FindWithTag("Player").GetComponent<PlayerSwitcher>().DisableMeshSwitch();
	}

	void Respawn()
	{
		for (int i = 0; i < marbles.Length; i++)
		{
			marbles[i].SetActive(true);
		}

		collected = 0;
		marbleText.text = "0 / " + marbles.Length;
	}
}
