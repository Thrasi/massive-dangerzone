using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class DiscreteVehicles : MonoBehaviour {

	private static List<Color> colors = new List<Color> {
		Color.red, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta,
		Color.yellow, Color.white, Color.black
	};

	private const int F = 20;
	private int c = 0;

	// Object to use for vehicles
	public GameObject vehicle;

	// Object for obstacle
	public GameObject obstacle;

	// Name of the file which contains a map
	public string filename;

	// List of obstacle gameobjects and the parent
	private GameObject obstacleParent;
	private List<GameObject> obstacles;

	// List of vehicles
	private List<GameObject> vehicles;

	// Path of the solution
	private IEnumerable<State> path;
	private IEnumerable<State> closed;

	// Use this for initialization
	void Start () {
		DiscreteMap map = new DiscreteMap("Assets/_Data/" + filename);
		/*foreach (var t in map.Successors(new State(new Vector3[] {
			new Vector3(1, 0, 1), new Vector3(4, 0, 2)
		}))) {
			print("point " + t);
		}*/
		List<Vector3> obstaclePositions = map.GetObstaclePositions();
		GenerateObstacles(obstaclePositions);
		List<Vector3> vehiclePositions = map.GetStartPositions();
		GenerateVehicles(vehiclePositions);
		AStar ast = new AStar(map);
		path = ast.path;
		closed = ast.closed;
/*		foreach (State s in path) {
			print(s);
		}
			
		*/
	}
	
	// Update is called once per frame
	void Update () {
		return;
		c++;
		if (c >= F) {
			c = 0;
			if (path.Count() == 0) {
				return;
			}
			Vector3[] positions = path.First().positions;
			path = path.Skip(1);
			for (int i = 0; i < positions.Length; i++) {
				vehicles[i].transform.position = positions[i];
			}
		}
	}

	// Generates obstacles in map
	private void GenerateObstacles(List<Vector3> positions) {
		obstacleParent = new GameObject();
		obstacleParent.name = "Obstacles";
		obstacles = new List<GameObject>();
		foreach (Vector3 pos in positions) {
			GameObject obj = Instantiate(obstacle) as GameObject;
			obj.transform.position = pos;
			obj.transform.parent = obstacleParent.transform;
			obj.SetActive(true);
			obstacles.Add(obj);
		}
	}

	// Generates vehicle objects in the scene
	private void GenerateVehicles(List<Vector3> positions) {
		vehicles = new List<GameObject>();
		System.Random rnd = new System.Random();
		foreach (Vector3 pos in positions) {
			GameObject obj = Instantiate(vehicle) as GameObject;
			obj.transform.position = pos;
			obj.transform.parent = transform;
			obj.renderer.material.color = colors[rnd.Next(colors.Count)];
			obj.SetActive(true);
			vehicles.Add(obj);
		}
	}

	void OnDrawGizmos() {
		if (closed != null) {
			Gizmos.color = Color.red;
			foreach (State s in closed) {
				foreach (Vector3 v in s.positions) {

					Gizmos.DrawSphere(v, 0.5f);
				}
			}
		}
	}
}
