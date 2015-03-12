using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Polygon {

	// Vertices list
	private List<Vector3> vertices;

	// Edge collection, not really needed but sacrificing some memory
	// for better time complexity
	private HashSet<Edge> edges;


	// Constructor creates a list of edges and vertices
	public Polygon(IEnumerable<Vector3> vs) {
		vertices = new List<Vector3>();
		foreach (Vector3 v in vs) {
			vertices.Add(v);
		}
		
		int n = vertices.Count;
		edges = new HashSet<Edge>();
		for (int i = 0; i < n-1; i++) {
			edges.Add(new Edge(vertices[i], vertices[i+1]));
		}
		edges.Add(new Edge(vertices[0], vertices[n-1]));
	}

	// Returns an enumerable of vertices
	public IEnumerable<Vector3> Vertices() {
		return vertices;
	}

	// Returns an inumerable of edges
	public IEnumerable<Edge> Edges() {
		return edges;
	}

	// Checks if the polygon contains the edge
	public bool ContainsEdge(Edge e) {
		return edges.Contains(e);
	}

	// Checks if the polygon contains the vertex
	public bool ContainsVertex(Vector3 v) {
		return vertices.Contains(v);
	}

	// Check if the polygon intersects with arc
	public bool Intersects(Arc arc) {
		foreach (Edge e in edges) {
			if (arc.Intersects(e)) {
				return true;
			}
		}
		return false;
	}

	// Checks if the edge intersects with the polygon
	public bool Intersects(Edge line) {
		foreach (Edge e in edges) {
			if (e.Intersect(line)) {
				return true;
			}
		}
		return false;
	}

	// Check if point is inside polygon
	// Using RayCaster algorithm
	public bool IsInside(Vector3 p) {
		int count = 0;
		Edge ray = new Edge(p, new Vector3(float.MaxValue, 0, p.z));
		foreach (Edge e in edges) {
			if (ray.Intersect(e)) {
				count++;
			}
		}
		return count % 2 == 1;
	}

	// Finds all intersections of this polygon and an edge
	public List<Vector3> Intersections(Edge edge) {
		List<Vector3> points = new List<Vector3>();
		foreach (Edge e in edges) {
			Vector3? interPoint = edge.Intersection(e);
			if (interPoint.HasValue) {
				points.Add(interPoint.Value);
			}
		}
		return points;
	}

	// Get iterator for edges
	public IEnumerable<Edge> IterEdges() {
		return edges;
	}

	// Not my function
	// Creates a game object for this polygon
	// I have added UVs
	public GameObject ToGameObject(Material mat) {
		// Create Vector2 vertices
		Vector2[] vertices2D = new Vector2[this.vertices.Count];
        for (int i = 0; i < this.vertices.Count; i++) {
        	vertices2D[i] = new Vector2(this.vertices[i].x, this.vertices[i].z);
        }
 
        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();
 
        // Create the Vector3 vertices
        Vector3[] vert = new Vector3[vertices2D.Length];
        for (int i=0; i<vert.Length; i++) {
            vert[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
        }

        // Create UVs
        Vector2[] uvs = new Vector2[vert.Length];
        for (int i = 0; i < vert.Length; i++) {
        	uvs[i].x = Mathf.Sin((float)i/vert.Length);
        	uvs[i].y = Mathf.Cos((float)i/vert.Length);
        }
 
        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vert;
        msh.uv = uvs;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();
 
        // Set up game object with mesh;
        GameObject go = new GameObject();
        go.name = "PolygonObstacle";
        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        mr.material = mat;
        MeshFilter filter = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
        return go;
	}
}
