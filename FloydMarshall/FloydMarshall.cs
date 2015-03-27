using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class FloydMarshall {
	public IList<VertexG> adjacencyList;
	private int counter = 0;
	public int V = 0;
	public float[,] dist;
	public int[,] next;
	
	public FloydMarshall() {
		adjacencyList = new List<VertexG>();
	}
	
	public void Add(VertexG v) {
		if ( !adjacencyList.Contains(v) ) {
			adjacencyList.Add(v);
			v.number = counter++;
			V = counter;
		}
	}

	public VertexG getVertex(int index) {
		return adjacencyList[index];
	}
	
	public void AddEdge(VertexG v, VertexG u, float w) {
		if ( !adjacencyList.Contains(v) ) {
			return;
		}

		int index = adjacencyList.IndexOf (u);
//		Debug.Log ("Vertex: "+v+", adding: "+u);
		if (index < 0) {
			Add (u);
		} else {
			u.number = index;
		}
		VertexG vv = adjacencyList [adjacencyList.IndexOf(v)];
		vv.neighbours.Add (u, w);
	}
	
	public bool Contains(VertexG v) {
		return adjacencyList.Contains (v);
	}
	
	public float distance(VertexG v, VertexG u) {
		int i = adjacencyList.IndexOf (v);
		int j = adjacencyList.IndexOf (u);
//		Debug.Log (string.Format("v: {3}, u: {4}, i: {0}, j: {1}, V: {2}, distance: {5}", i, j, V, v, u,dist[i,j]));
		return dist[i,j];
	}

	// FLOYD MARSHALL ALGORITHM
	public void calcDists() {
		
		dist = new float[V,V];
		next = new int[V,V];
		for (int i=0; i<V; i++) {
			for(int j=0; j<V; j++) {
				dist[i,j] = float.MaxValue;
				next[i,j] = -1;
			}		
		}
		
		foreach (VertexG l in adjacencyList) {
			int u = l.number;
//			Debug.Log("Vertex: "+l+", number: "+u+", length of neighbours: "+l.neighbours.Count);
//			IDictionary<VertexG, float> neighs = l.neighbours;

			foreach (KeyValuePair<VertexG, float> edge in l.neighbours) {
				int v = edge.Key.number;
				dist[u,v] = edge.Value;
				next[u,v] = v;
			}
		}
		int iffed = 0;
		for (int k=0; k<V; k++) {
			for (int i=0; i<V; i++) {
				for (int j=0; j<V; j++) {
					if ( dist[i,k] + dist[k,j] < dist[i,j] ) {
						iffed+=1;
						dist[i,j] = dist[i,k] + dist[k,j];
						next[i,j] = next[i,k];
					}
				}
			}
		}
		Debug.Log ("iffed: " + iffed);


	}

	public List<Vector3> Path(int u, int v) {
		List<int> path = new List<int>();
		List<Vector3> p = new List<Vector3> ();
		if (next[u,v] == -1 ) {
			return p;
		}
		path.Add (u);
		p.Add (adjacencyList [u].pos);
		while (u != v) {
			u = next[u,v];
			path.Add (u);
			p.Add (adjacencyList [u].pos);
		}
		return p;
	}
}

public class VertexG {
	public Vector3 pos;
	public int number;
	public IDictionary<VertexG,float> neighbours;
	

	public VertexG(Vector3 pos) {
		this.pos = pos;
		this.neighbours = new Dictionary<VertexG,float>();
	}

	public override bool Equals(object other) {
		if (!(other is VertexG)) {
			return false;
		}
		VertexG obj = other as VertexG;
		return this.pos == obj.pos;
	}
	
	// For dictionary
	public override int GetHashCode() {
		return pos.GetHashCode();
	}
	
	// For debugging
	public override string ToString() {
		return string.Format("({0})", pos);
	}
}




























