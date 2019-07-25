using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CentipedeBoss : MonoBehaviour {

	public delegate void CentipedeEvent (CentipedeBoss c);

	public event CentipedeEvent FinishPath;
	public event CentipedeEvent ReachDestination;

	public Transform[] nodes;
	public Animator[] myAnims;
	public SortingGroup myGroup;

	public float moveSpeed;
	// maximum angle the centipede can change its direction in a seconds
	public float rotateSpeed;

	// this array stores the distance between each node and the head
	float[] distanceToHead;

	//
	List<NodeData> path = new List<NodeData>();
	// used for CHASE state
	Vector3 currentDirection;
	Transform player;
	// used for PATH state
	Stack<Vector3> myPath = new Stack<Vector3>();
	// used for RETURN state
	Vector3 destination;

	enum AI_STATE {
		NONE,
		PATH,
		CHASE,
		RETURN
	}

	AI_STATE state;

	public void SetPathState (List<Vector3> path) {
		state = AI_STATE.PATH;
		myGroup.sortingOrder = -2;
		moveSpeed = 6;
		rotateSpeed = 6 * Mathf.Deg2Rad;
		myPath.Clear();
		for (int i = path.Count - 1; i >= 0; i--) {
			myPath.Push(path[i]);
		}
		currentDirection = myPath.Peek() - nodes[0].localPosition;
		currentDirection.Normalize();
		this.path.Clear();
		this.path.Add(new NodeData(nodes[0].localPosition, 0));
	}

	public void SetChaseState (float moveSpeed, float rotateSpeed) {
		state = AI_STATE.CHASE;
		myGroup.sortingOrder = 0;
		this.moveSpeed = moveSpeed;
		this.rotateSpeed = rotateSpeed * Mathf.Deg2Rad;
	}
		
	public void SetReturnState (Vector3 destination) {
		state = AI_STATE.RETURN;
		myGroup.sortingOrder = -2;
		moveSpeed = 6;
		rotateSpeed = 6 * Mathf.Deg2Rad;
		this.destination = destination;
	}

	void Start () {
		player = GameManager.Instance.player1.transform;
		// init the distance to the head of each node
		distanceToHead = new float[nodes.Length];
		for (int i = 1; i < distanceToHead.Length; i++) {
			distanceToHead[i] = 0.1f + 0.3f * (i - 1);
		}
	}

	void Update () {
		if (state == AI_STATE.PATH) {
			MoveOnPath();
		} else if (state == AI_STATE.CHASE) {
			ChasePlayer();
		} else if (state == AI_STATE.RETURN) {
			MoveToPosition(destination);
		}
	}

	void OnEnable () {
		// init the anim delay time for each body part
		for (int i = 0; i < myAnims.Length; i++) {
			myAnims[i].SetFloat(AnimConst.delay, 1 - 0.1f * i);
		}
	}

	// used for PATH state
	void MoveOnPath () {
		if (myPath.Count > 0) {
			Vector3 dir = myPath.Peek() - nodes[0].localPosition;
			float amount = Time.deltaTime * moveSpeed;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
			nodes[0].localRotation = Quaternion.Euler(0, 0, angle);
			nodes[0].localPosition = Vector3.MoveTowards(nodes[0].localPosition, myPath.Peek(), amount);
			if (nodes[0].localPosition == myPath.Peek()) {
				myPath.Pop();
				if (myPath.Count == 0) {
					if (FinishPath != null)
						FinishPath(this);
				}
			}
			path.Insert(0, new NodeData(nodes[0].localPosition, amount));
			UpdateAllBodyParts();
		}
	}

	// used for CHASE state
	void MoveAhead (Vector3 dir) {
		float amount = Time.deltaTime * moveSpeed;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
		nodes[0].localRotation = Quaternion.Euler(0, 0, angle);
		nodes[0].localPosition += dir.normalized * amount;
		path.Insert(0, new NodeData(nodes[0].localPosition, amount));
		UpdateAllBodyParts();
	}

	// used for RETURN state
	void MoveToPosition (Vector3 pos) {
		Vector3 dir = pos - nodes[0].localPosition;
		float amount = Time.deltaTime * moveSpeed;
		if (dir.x == 0 && dir.y == 0) {
			nodes[0].localPosition += Vector3.forward * amount;
		} else {
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
			nodes[0].localRotation = Quaternion.Euler(0, 0, angle);
			nodes[0].localPosition = Vector3.MoveTowards(nodes[0].localPosition, pos, amount);
		}
		path.Insert(0, new NodeData(nodes[0].localPosition, amount));
		UpdateAllBodyParts();
		if (nodes[nodes.Length - 1].localPosition.x == pos.x && nodes[nodes.Length - 1].localPosition.y == pos.y) {
			if (ReachDestination != null)
				ReachDestination(this);
		}
	}

	void UpdateAllBodyParts () {
		int curPathId = 0;
		float sumLength = 0;
		for (int i = 1; i < nodes.Length; i++) {
			int temp = curPathId;
			for (int j = temp; j < path.Count; j++) {
				if (path[j].distanceToNextNode + sumLength < distanceToHead[i]) {
					sumLength += path[j].distanceToNextNode;
					if (curPathId == path.Count - 1) {
						nodes[i].localPosition = path[curPathId].position;
					} else
						curPathId++;
				} else {
					float diff = distanceToHead[i] - sumLength;
					nodes[i].localPosition = Vector3.Lerp(path[j].position, path[j + 1].position, diff / path[j].distanceToNextNode);
					Vector3 dire = path[j + 1].position - path[j].position;
					float angle2 = Mathf.Atan2(dire.y, dire.x) * Mathf.Rad2Deg;
					nodes[i].localRotation = Quaternion.Euler(0, 0, angle2);
					break;
				}
			}
		}
		// remove unused path
		if (curPathId + 2 < path.Count)
			path.RemoveRange(curPathId + 2, path.Count - curPathId - 2);
	}

	void FollowMouse () {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;
		Vector3 dir = mousePos - nodes[0].localPosition;
		currentDirection = Vector3.RotateTowards(currentDirection, dir, rotateSpeed, 0);
		MoveAhead(currentDirection);
	}

	void ChasePlayer () {
		Vector3 dir = Vector3.zero;
		dir = player.localPosition - nodes[0].localPosition;
		currentDirection = Vector3.RotateTowards(currentDirection, dir, rotateSpeed, 0);
		MoveAhead(currentDirection);
	}
}

public class NodeData {
	public Vector3 position;
	public float distanceToNextNode;

	public NodeData (Vector3 pos, float dis) {
		position = pos;
		distanceToNextNode = dis;
	}
}