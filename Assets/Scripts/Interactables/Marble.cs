using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
	[SerializeField] float dist;
	Transform player;

	Renderer rend;
	MarbleManager marbleManager;

	void Start()
	{
		player = GameObject.FindWithTag("Player").transform;
		marbleManager = GameObject.Find("MarbleManager").GetComponent<MarbleManager>();
		rend = GetComponent<Renderer>();

		rend.material.color = new Color(Random.Range(0, 100) * 0.01f,
			Random.Range(0, 100) * 0.01f, Random.Range(0, 100) * 0.01f);
	}

	void Update()
	{
		if (Vector3.Distance(player.position, transform.position) < dist)
		{
			marbleManager.GetMarble();
			gameObject.SetActive(false);
		}
	}
}
