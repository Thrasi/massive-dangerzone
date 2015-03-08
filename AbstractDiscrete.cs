using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractDiscrete : AbstractVehicles {

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
		obstacle = Resources.Load("GameObjects/Obstacle") as GameObject;
		vehicle = Resources.Load("GameObjects/Vehicle") as GameObject;

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
