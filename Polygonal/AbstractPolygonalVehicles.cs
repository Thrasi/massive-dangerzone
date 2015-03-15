using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ObstaclesExt;

public class AbstractPolygonalVehicles : AbstractVehicles {

	// To radians
	protected const float toRad = Mathf.PI / 180;

	// TO degrees
	protected const float toDeg = 180 / Mathf.PI;


	// List of polygonal obstacles
	protected List<Polygon> obstacles;

	// Material for obstacles
	protected Material material;

	// Generate and render all obstacles
	protected void GenerateObstacles(IEnumerable<Polygon> polys) {
		GameObject parent = new GameObject();
		parent.name = "Polygonal Obstacles";
		foreach (Polygon pol in polys) {
			GameObject go = pol.ToGameObject(material);
			go.transform.parent = parent.transform;
		}
	}
	
}
