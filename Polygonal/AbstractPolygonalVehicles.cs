using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AbstractPolygonalVehicles : AbstractVehicles {

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

	// Finds the closest edge in obstacles to given point
	// Returns both the edge and distance to that edge
	// If there are no obstacles, it returns tuple which has Edge set as null
	protected Tuple<Edge, float> ClosestWall(Vector3 point) {
		float minDist = float.MaxValue;
		Edge minEdge = null;
		foreach (Polygon pol in obstacles) {
			foreach (Edge e in pol.Edges()) {
				float dist = e.Distance(point);
				if (dist < minDist) {
					minDist = dist;
					minEdge = e;
				}
			}
		}
		return Tuple.Create(minEdge, minDist);
	}

	// Finds all intersections of given edge and obstacles
	protected List<Vector3> Intersections(Edge edge) {
		List<Vector3> points = new List<Vector3>();
		foreach (Polygon pol in obstacles) {
			points.AddRange(pol.Intersections(edge));
		}
		return points;
	}

	// Finds distance to closest intersection of the edge and obstacles
	// Returns MaxValue if it doesnt intersect
	// Position here is redundant but I use it to clear confusion with Edge
	protected float DistToClosest(Edge edge, Vector3 pos) {
		List<Vector3> inters = Intersections(edge);
		float distClosest = float.MaxValue;
		if (inters.Count > 0) {
			distClosest = inters.Min(p => Vector3.Distance(p, pos));
		}
		return distClosest;
	}
}
