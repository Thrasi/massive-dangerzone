using UnityEngine;
using System.Collections.Generic;

public class VRPPolygonalVehicles : AbstractVehicles {


	public string filename;

	private Material material;

	// Use this for initialization
	void Start () {
		material = Resources.Load("Materials/ObstacleMaterial") as Material;
		vehicle = Resources.Load("GameObjects/LargeVehicle") as GameObject;
		VRPPolygMap map = new VRPPolygMap("Assets/_Data/VRPPolyg/" + filename);
		GenerateObstacles(map.polys);
		GenerateVehicles(new List<Vector3>(map.starts));
	}
	
	// Update is called once per frame
	void Update () {
	}

	// Generate and render all obstacles
	private void GenerateObstacles(IEnumerable<Polygon> polys) {
		GameObject parent = new GameObject();
		parent.name = "Polygonal Obstacles";
		foreach (Polygon pol in polys) {
			GameObject go = pol.ToGameObject(material);
			go.transform.parent = parent.transform;
		}
	}

}
