using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractVehicles : MonoBehaviour {

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

	// List of vehicles
	protected List<GameObject> vehicles;
	protected GameObject vehicle;


	// Nothing here
	void Start() {
	}

	// Nothing here
	void Update() {
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
}
