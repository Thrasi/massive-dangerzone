using UnityEngine;
using System.Collections;

public class VRPPolygonalVehicles : MonoBehaviour {

	public string filename;

	private Material material;

	// Use this for initialization
	void Start () {
		VRPPolygMap map = new VRPPolygMap("Assets/_Data/VRPPolyg/" + filename);
		material = Resources.Load("Materials/ObstacleMaterial") as Material;

		// Generate and render all obstacles
		GameObject parent = new GameObject();
		parent.name = "Polygonal Obstacles";
		foreach (Polygon pol in map.polys) {
			GameObject go = pol.ToGameObject(material);
			go.transform.parent = parent.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
