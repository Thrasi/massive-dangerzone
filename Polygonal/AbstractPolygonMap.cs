using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractPolygonMap {

	// Number of vehicles
	public int N { get; protected set; }

	// Start positions of vehicles
	public Vector3[] starts { get; protected set; }

	// Polygonal obstacles
	public List<Polygon> polys { get; protected set; }

	// Returns list of positions of vehicles
	public List<Vector3> GetVehiclePositions() {
		return new List<Vector3>(starts);
	}
}
