using UnityEngine;
using System.Collections;

public class ObstacleAvoidanceEmpty : AbstractVehicles {

	public string filename;

	// Use this for initialization
	void Start () {
		vehicle = Resources.Load("GameObjects/LargeVehicle") as GameObject;
		PolygonMap map = new PolygonMap("Assets/_Data/ColAvoidMaze/" + filename);
		GenerateVehicles(map.GetVehiclePositions());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
