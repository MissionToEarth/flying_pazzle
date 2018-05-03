using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resource_load : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		//Asset内にあるresourcesフォルダから、Textureを読み込む。
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
		Renderer rend = go.GetComponent<Renderer>();
		rend.material.mainTexture = Resources.Load("glass") as Texture;		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
