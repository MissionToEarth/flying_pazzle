using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Utilities;


public class FallingBlockPuzzleManager : MonoBehaviour
{
	enum PHASE
	{
		INIT,
		LOAD,
		START,
		DROP,
		USER_TURN,
		SCAN,
		END,
	};

	private PHASE phase = PHASE.INIT;

	// Blockオブジェクト
	[SerializeField] private PuzzleBlock prefab_block = null;
	// 盤面の行数列数
	[SerializeField] private int row_max = 1;
//行
	[SerializeField] private int col_max = 1;
//列

	// 動作中のブロックの一覧
	private List<PuzzleBlock> block_list = new List<PuzzleBlock> ();

	// 盤面
	private int[,] table;
	public Vector2 TableSize {
		get;
		set;
	}
	//盤面の画面のサイズ。
	private float grid_width = 0;
	private float grid_height = 0;

	void dump ()
	{
		Debug.LogFormat ("phase:{0} row:{1} col:{2} grid width:{3} grid height:{4} active block num:{5}",
			phase,
			row_max,
			col_max,
			grid_width,
			grid_height,
			block_list.Count
		);
	}

	// awake
	void Awake ()
	{
		this.phase = PHASE.INIT;

		this.table = new int[row_max, col_max];
		for (int i = 0; i < row_max; i++)
		{
			for (int j = 0; j < col_max; j++)
			{
				this.table[i, j] = 0;
			}
		}
		foreach (int i in table)
		{
			Debug.LogFormat ("{0}", i);
		}

		// カメラの左下座標を取得
		Vector2 min = Window.GetWorldMin ();
		// カメラの右上座標を取得する
		Vector2 max = Window.GetWorldMax ();	
		this.TableSize = max - min;

		this.grid_width = this.TableSize.x / this.col_max;
		this.grid_height = this.TableSize.y / this.row_max;


		#if DEBUG
		dump ();
		#endif
	}

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		Debug.Log ("Update");
		switch (this.phase)
		{
		case PHASE.INIT:
			this.phase = PHASE.START;
			break;
		case PHASE.LOAD:
			break;
		case PHASE.START:
			Debug.Log ("Update");
			bool finish = ShowStart ();
			if (finish)
			{
				this.phase = PHASE.DROP;
			}
			break;
		case PHASE.DROP:
			if (this.block_list.Count <= 0)
			{
				Debug.Log ("Update");
				BlockDrop ();
			} else
			{
				MoveBlock ();
			}
			break;
		case PHASE.USER_TURN:
//			Puzzle.update ();
			this.phase = PHASE.DROP;
			break;
		case PHASE.SCAN:
//			Puzzle.Scan ();
			break;
		case PHASE.END:
			break;
		default:
			break;
		}
	}

	bool show_text = false;

	bool ShowStart ()
	{
		this.show_text = true;

		return true;
	}

	void BlockDrop ()
	{
		Debug.Log ("On Drop ");
		//条件に沿ってブロックを投入
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			Debug.Log ("drop");

			PuzzleBlock gm = Instantiate (prefab_block, this.transform) as PuzzleBlock;
			gm.transform.position = new Vector3 ((float)this.grid_width * i, (float)this.grid_height * row_max, 0);

			this.block_list.Add (gm);
		}
	}

	void MoveBlock ()
	{
		Debug.Log ("MoveBlock");

		float dest_x = 0;
		float dest_y = -0.062f;
		foreach (PuzzleBlock block in this.block_list.ToArray())
		{
			block.Move (dest_x, dest_y);
			if (HitBlock (block))
			{
				Debug.Log ("HIT BLOCK");
				block.Move (-dest_x, -dest_y);
				this.block_list.Remove (block);

				this.table[GetGridRowIndex (block), GetGridColIndex (block)] = 1;
			}
			else if (HitGround (block))
			{
				Debug.Log ("HIT GOUND");
				block.Move (-dest_x, -dest_y);
				this.block_list.Remove (block);

				this.table[GetGridRowIndex (block), GetGridColIndex (block)] = 1;
			}

			if (this.block_list.Count <= 0)
			{
				this.phase = PHASE.USER_TURN;
			}
		}
	}

	/// <summary>
	/// Gets the index of the grid row.
	/// </summary>
	/// <returns>The grid row index.</returns>
	/// <param name="block">Block.</param>
	int GetGridRowIndex(PuzzleBlock block)
	{
		Vector3 vec = (Vector2)block.transform.position - Window.GetWorldMin();
		int row_idx = (int)(vec.x / this.grid_width);
		row_idx = Mathf.Clamp (row_idx, 0, row_max - 1);

		return row_idx;
	}

	/// <summary>
	/// Gets the index of the grid col.
	/// </summary>
	/// <returns>The grid col index.</returns>
	/// <param name="block">Block.</param>
	int GetGridColIndex(PuzzleBlock block)
	{
		Vector2 vec = (Vector2)block.transform.position - Window.GetWorldMin();
		int col_idx = (int)(vec.y / this.grid_width);
		col_idx = Mathf.Clamp (col_idx, 0, col_max - 1);

		return col_idx;
	}

	/// <summary>
	/// Hits the block.
	/// </summary>
	/// <returns><c>true</c>, if block was hit, <c>false</c> otherwise.</returns>
	/// <param name="block">Block.</param>
	bool HitBlock (PuzzleBlock block)
	{
		Vector3 vec = (Vector2)block.transform.position - Window.GetWorldMin();
		int row_idx = (int)(vec.x / this.grid_width);
		int col_idx = (int)(vec.y / this.grid_height);

		Debug.LogFormat ("RAW : [row {0} : col {1}]", row_idx, col_idx);

		row_idx = Mathf.Clamp (row_idx, 0, row_max - 1);
		col_idx = Mathf.Clamp (col_idx, 0, col_max - 1);

		Debug.LogFormat ("Clamp : [row {0} : col {1}]", row_idx, col_idx);

		return (this.table[row_idx, col_idx] != 0);
	}

	bool HitGround (PuzzleBlock block)
	{
		Debug.LogFormat ("height : {0}", block.transform.position.y - this.grid_height * 0.5);
		return Window.GetWorldMin ().y >= (block.transform.position.y - this.grid_height * 0.5);
	}


	void OnGUI ()
	{
		if (this.show_text)
		{
			// 敵が全滅した
			// フォントサイズ設定
			Util.SetFontSize (32);
			// 中央揃え
			Util.SetFontAlignment (TextAnchor.MiddleCenter);

			// フォントの位置
			float w = 128; // 幅
			float h = 32; // 高さ
			float px = Screen.width / 2 - w / 2;
			float py = Screen.height / 2 - h / 2;

			// フォント描画
			Util.GUILabel (px, py, w, h, "Game Clear!");

//			// ボタンは少し下にずらす
//			py += 60;
//			if (GUI.Button(new Rect(px, py, w, h), "Back to Title"))
//			{
//				// タイトル画面にもどる
//				Application.LoadLevel("Title");
//			}
		}
	}

}
