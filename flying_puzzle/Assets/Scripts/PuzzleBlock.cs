using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBlock : Token {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void Move(float x, float y)
	{
		Debug.Log ("Is Move!?");

		Vector3 pos = transform.position;
		pos.x += x;
		pos.y += y;
		transform.position = pos;
	}
}
