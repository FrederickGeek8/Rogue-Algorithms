using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AIScript : Player {

	private List <Tuple> openStack = new List<Tuple> ();
	private List <Tuple> closedStack = new List<Tuple> ();
	private Dictionary<Tuple, int> gScores = new Dictionary<Tuple, int> ();
	private Dictionary<Tuple, Tuple> cameFrom = new Dictionary<Tuple, Tuple> ();

	protected override void Start () {
		Vector2 pos = transform.position;
		Tuple pos2 = new Tuple(new int[] { (int) pos.x, (int) pos.y });
		openStack.Add (pos2);
		gScores.Add (pos2, 0);
		base.Start ();
	}

	protected override void Update () {
		Vector2 pos = transform.position;
		if (!GameManager.instance.playerTurn) {
			return;
		}

		int horizontal = 0;
		int vertical = 0;

		// do computations here for the next move
		int[] nextPos = calculateNextMove();

		if (nextPos == null) {
			throw new System.NullReferenceException ("No path can be found!");
		}

		horizontal = nextPos [0] - (int) pos.x;
		vertical = nextPos [1] - (int) pos.y;

		if (horizontal != 0) {
			vertical = 0;
		}

		if (horizontal != 0 || vertical != 0) {
			base.AttemptMove<Wall> (horizontal, vertical);
		}
	}

	private int[] calculateNextMove () {
		Vector2 pos = transform.position;
		Tuple pos2 = new Tuple(new int[] { (int) pos.x, (int) pos.y });

		Tuple exit = new Tuple(new int[] { GameManager.instance.boardScript.columns - 1, GameManager.instance.boardScript.rows - 1 });

		Tuple calculatedPos = pos2;
		while (openStack.Count > 0) {
			openStack = openStack.OrderBy (it => (MinkowskiDistance(it.values, pos2.values) + gScores[it])).ToList();
			Tuple minF = openStack [0];

			calculatedPos = minF;

			if (minF.Equals(exit)) {
				List<Tuple> totalPath = new List<Tuple>();
				totalPath.Add (calculatedPos);

				Tuple current = calculatedPos.Clone ();
				while (cameFrom.Keys.Contains(current)) {
					current = cameFrom [current];
					totalPath.Add (current);
				}

				totalPath.Reverse ();

				openStack.Clear ();
				closedStack.Clear ();
				gScores.Clear ();
				cameFrom.Clear ();

				openStack.Add (pos2);
				gScores.Add (pos2, 0);

				return totalPath[1].values;
			}

			openStack.Remove (calculatedPos);
			closedStack.Add (calculatedPos);

			List<int[]> openPath = getAvailableSteps (calculatedPos.values);
			foreach (int[] rawItem in openPath) {
				Tuple item = new Tuple (rawItem);
				if (closedStack.Contains (item)) {
					continue;
				}

				if (!openStack.Contains(item)) {
					openStack.Add(item);
				}

				int tentative_gScore = gScores [calculatedPos] + 1;
				if (gScores.Keys.Contains (item) && tentative_gScore >= gScores [item]) {
					continue;
				}

				gScores [item] = tentative_gScore;
				cameFrom [item] = calculatedPos;
			}
		}

		openStack.Clear ();
		closedStack.Clear ();
		gScores.Clear ();
		cameFrom.Clear ();

		openStack.Add (pos2);
		gScores.Add (pos2, 0);

		return null;
	}

	private List<int[]> getAvailableSteps(int[] pos) {
		int x = pos [0];
		int y = pos [1];
		int width = GameManager.instance.boardScript.columns;
		int height = GameManager.instance.boardScript.rows;
		GameObject[] tiles = GameManager.instance.boardScript.tiles;

		List<int[]> available = new List<int[]> ();

		int[] xMin1 = new int[] { x - 1, y };
		if (x - 1 > -1 && canMove(pos, xMin1)) {
			available.Add (xMin1);
		}

		int[] xPlus1 = new int[] { x + 1, y };
		if (x + 1 < width && canMove(pos, xPlus1)) {
			available.Add (xPlus1);
		}

		int[] yMin1 = new int[] { x, y - 1 };
		if (y - 1 > -1 && canMove(pos, yMin1)) {
			available.Add (yMin1);
		}

		int[] yPlus1 = new int[] { x, y + 1 };
		if (y + 1 < height && canMove(pos, yPlus1)) {
			available.Add (yPlus1);
		}

		return available;
	}

	private bool canMove(int[] pos, int[] target) {
		RaycastHit2D hit;
		Vector2 start = new Vector2(pos[0], pos[1]);
		Vector2 end = new Vector2(target[0], target[1]);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			return true;
		}

		return false;
	}

	private float MinkowskiDistance (int[] first, int[] second, float p = 1f) {
		float value = 0f;
		for (int i = 0; i < first.Length; i++) {
			value += Mathf.Pow (Mathf.Abs (first [i] - second [i]), p);
		}

		return value;
	}
}