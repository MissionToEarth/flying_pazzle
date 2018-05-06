using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
	
public class Window : MonoBehaviour 
{
	/// 画面の左下のワールド座標を取得する.
	static public Vector2 GetWorldMin()
	{
		return  Camera.main.ViewportToWorldPoint (Vector2.zero);
	}

	/// 画面右上のワールド座標を取得する.
	static public Vector2 GetWorldMax()
	{
		return Camera.main.ViewportToWorldPoint (Vector2.one);
	}

}

}