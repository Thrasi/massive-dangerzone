using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class DiscreteVehicles : MonoBehaviour {

	private static List<Color> colors = new List<Color> {
		Color.red,    Color.blue,  Color.cyan,
		Color.gray,   Color.green, Color.magenta,
		Color.yellow, Color.white, Color.black
	};

	// Variables for moving vehicles
	private const int F = 20;		// Every 20 frames
	private int c = 0;				// Counter
	private int step = 0;			// Step in the time

	// For label
	private float cost;
	private GUIStyle labelStyle;
	private Rect labelRect;
	private string strCost;

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
	private List<State>[] paths;

	
	// Read the map and run astar
	void Start () {
		DiscreteMap map = new DiscreteMap("Assets/_Data/" + filename);
		
		List<Vector3> obstaclePositions = map.GetObstaclePositions();
		GenerateObstacles(obstaclePositions);
		List<Vector3> vehiclePositions = map.GetStartPositions();
		GenerateVehicles(vehiclePositions);
		
		AStar ast = new AStar(
			map,
			delegate(Vector3 a, Vector3 b) {
				return (a-b).magnitude;
			}
		);
		paths = ast.paths;
		cost = ast.cost;

		// Initialize label printing
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		labelRect = new Rect(20, 20, 20, 20);
		strCost = "Time: " + cost.ToString("0.00");
	}
	
	// Move all vehicles one step
	void Update () {
		c++;
		if (c >= F) {
			c = 0;
			for (int i = 0; i < paths.Length; i++) {
				if (step < paths[i].Count) {
					vehicles[i].transform.position = paths[i][step].pos;
				}
			}
			step++;
		}
	}

	// Show the label on screen
	void OnGUI() {
		GUI.Label(labelRect, strCost, labelStyle);
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

	// Draws paths
	void OnDrawGizmos() {
		if (paths != null) {
			for (int i = 0; i < paths.Length; i++) {
				Gizmos.color = vehicles[i].renderer.material.color;
				int len = paths[i].Count;
				for (int j = 0; j < len-1; j++) {
					Gizmos.DrawSphere(paths[i][j].pos, 0.05f);
					Gizmos.DrawLine(paths[i][j].pos, paths[i][j+1].pos);
				}
				Gizmos.DrawSphere(paths[i][paths[i].Count-1].pos, 0.3f);
			}
		}
	}
}
