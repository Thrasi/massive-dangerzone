using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class DiscreteVehicles : MonoBehaviour {

	// Some colors for different vehicles
	private static List<Color> colors = new List<Color> {
		Color.red,    Color.blue,  Color.cyan,
		Color.gray,   Color.green, Color.magenta,
		Color.yellow, Color.white, Color.black,
		new Color(0.5f, 0.7f, 1.0f),
		new Color(0.7f, 0.5f, 1.0f),
		new Color(1.0f, 0.5f, 0.7f),
		new Color(1.0f, 0.7f, 0.5f),
		new Color(0.5f, 1.0f, 0.7f),
		new Color(0.7f, 1.0f, 0.5f)
	};

	// Variables for moving vehicles
	private const int F = 20;		// Every 20 frames
	private int c = 0;				// Counter
	private int step = 0;			// Step in the time

	// For label
	private float cost;
	private float startup;
	private GUIStyle labelStyle;
	private Rect labelRect;
	private string strCost;

	// Object to use for vehicles
	public GameObject vehicle;

	// Object for obstacle
	public GameObject obstacle;

	// Name of the file which contains a map
	public string filename;

	// Depth to explore
	public int depth;

	// List of obstacle gameobjects and the parent
	private GameObject obstacleParent;
	private List<GameObject> obstacles;

	// List of vehicles
	private List<GameObject> vehicles;

	// Path of the solution
	private List<State>[] paths;

	
	// Read the map and run astar
	void Start () {
		DiscreteMap map = new DiscreteMap("Assets/_Data/ColAvoidMaze/" + filename);
		
		List<Vector3> obstaclePositions = map.GetObstaclePositions();
		GenerateObstacles(obstaclePositions);
		List<Vector3> vehiclePositions = map.GetStartPositions();
		GenerateVehicles(vehiclePositions);

		AStar ast = new AStar(
			map,
			depth,
			delegate(Vector3 a, Vector3 b) {		// Heuristic function
				return (a-b).magnitude;
			}
		);
		paths = ast.paths;
		cost = ast.cost;
		startup = Time.realtimeSinceStartup;

		// Initialize label printing
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		labelRect = new Rect(20, 20, 20, 20);
		strCost = "Time: " + cost.ToString("0.00")
			+ "\nStartup: " + startup.ToString("0.00");
	}
	
	// Move all vehicles one step
	void Update () {
		c++;
		if (c >= F) {
			c = 0;
			int len = paths.Length;
			for (int i = 0; i < len; i++) {
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
	// Watch out, this function can be very slow for some reason
	// If depth is big then turn off sphere drawing
	void OnDrawGizmos() {
		if (paths != null) {
			for (int i = 0; i < paths.Length; i++) {
				Gizmos.color = vehicles[i].renderer.material.color;
				List<State> currPath = paths[i];
				for (int j = 0; j < cost; j++) {
					//Gizmos.DrawSphere(currPath[j].pos, 0.1f);
					Gizmos.DrawLine(currPath[j].pos, currPath[j+1].pos);
				}
				Gizmos.DrawSphere(currPath[(int) cost].pos, 0.3f);
			}
		}
	}
}
