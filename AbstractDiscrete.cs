using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractDiscrete : MonoBehaviour {

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

	// List of vehicles and gameobject to use for vehicle
	protected List<GameObject> vehicles;
	private GameObject vehicle;

	// List of obstacle gameobjects, parent and gameobject to use for obstacle
	protected GameObject obstacleParent;
	protected List<GameObject> obstacles;
	private GameObject obstacle;

	// Variables for moving vehicles
	protected const int F = 20;			// Every 20 frames
	protected int c = 0;				// Counter
	protected int step = 0;				// Step in the time

	// For label
	protected float cost;
	protected float startup;
	private GUIStyle labelStyle;
	private Rect labelRect;
	private string strCost;


	// Use this for initialization
	void Start () {
		// Load gameobjects from resources
		obstacle = Resources.Load("Obstacle") as GameObject;
		vehicle = Resources.Load("Vehicle") as GameObject;

		// Initialize label printing
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		labelRect = new Rect(20, 20, 20, 20);
		
		LocalStart();		// Wooo very important call
		
		startup = Time.realtimeSinceStartup;
		
		// Set string
		strCost = "Time: " + cost.ToString("0.00")
			+ "\nStartup: " + startup.ToString("0.00");
	}

	// Whatever subclass has to do
	protected virtual void LocalStart() {
	}

	// Update is not used, emphasize on not used in this class
	void Update () {
	}

	// Show the label on screen
	void OnGUI() {
		GUI.Label(labelRect, strCost, labelStyle);
	}

	// Generates vehicle objects in the scene
	protected void GenerateVehicles(List<Vector3> positions) {
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

	// Generates obstacles in map
	protected void GenerateObstacles(List<Vector3> positions) {
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
}
