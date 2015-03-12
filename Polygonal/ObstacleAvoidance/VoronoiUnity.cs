using UnityEngine;
using System.Collections.Generic;
using Voronoi2;

public class VoronoiUnity {

	// Some constant which I dont know at the moment what is the use
	private const double minDistanceBetweenSites = 0.1;

	// Reference to adaptee
	private Voronoi vor;

	// Obstacles needed for this voronoi
	private List<Polygon> obstacles;

	// Publicly availale edges
	public readonly List<Edge> edges;

	// Publicly available vertices
	public readonly List<Vector3> vertices;
	

	// Creates voronoi and generates all edges and vertices
	public VoronoiUnity(List<Vector3> points, List<Polygon> obstacles,
		float xmin, float xmax, float ymin, float ymax, bool connectAll) {
		
		this.obstacles = new List<Polygon>(obstacles);
		this.vor = new Voronoi(minDistanceBetweenSites);
		this.edges = new List<Edge>();
		this.vertices = new List<Vector3>();
		
		// Generate points
		double[] xs = new double[points.Count];
		double[] ys = new double[points.Count];
		for (int i = 0; i < points.Count; i++) {
			xs[i] = points[i].x;
			ys[i] = points[i].z;
		}
		
		// Generate voronoi and run through everything
		List<GraphEdge> gEdges = vor.generateVoronoi(xs, ys, xmin, xmax, ymin,ymax);
		foreach (GraphEdge e in gEdges) {
			Vector3 v1 = new Vector3((float)e.x1, 0, (float)e.y1);
			Vector3 v2 = new Vector3((float)e.x2, 0, (float)e.y2);
			bool ins1 = IsInside(v1);
			bool ins2 = IsInside(v2);
			
			// Adding vertices if they are not inside any of obstacles
			if (!ins1) { vertices.Add(v1); }
			if (!ins2) { vertices.Add(v2); }
			
			// Adding edges if they are not inside obstacles and do not intersect
			// with any of the obstacles, and only if connectAll is false
			Edge edge = new Edge(v1, v2);
			if (!connectAll && !Intersects(edge) && !ins1 && !ins2) {
				edges.Add(edge);
			}
		}

		// If connectAll is true then I will connect all vertices if possible
		int N = vertices.Count;
		if (connectAll) {
			for (int i = 0; i < N; i++) {
				for (int j = i+1; j < N; j++) {
					Vector3 v1 = vertices[i];
					Vector3 v2 = vertices[j];
					Edge e = new Edge(v1, v2);
					if (!Intersects(e)) {
						edges.Add(e);
					}
				}
			}
		}

	}// End constructor

	// Checks if the point is inside any of obstacles
	private bool IsInside(Vector3 v) {
		foreach (Polygon p in obstacles) {
			if (p.IsInside(v)) {
				return true;
			}
		}
		return false;
	}

	// Checks if edge intersects with any of the obstacles
	private bool Intersects(Edge e) {
		foreach (Polygon p in obstacles) {
			if (p.Intersects(e)) {
				return true;
			}
		}
		return false;
	}
}
