using UnityEngine;
using System.Collections.Generic;
using System;

public class DiscreteGraph : Graph {

	public readonly Tuple<int, int> size;
	private bool[,] map;

	public DiscreteGraph(bool[,] map, Tuple<int, int> size) {
		this.map = new bool[size._1, size._2];
		for (int y = 0; y < size._1; y++) {
			for (int x = 0; x < size._2; x++) {
				this.map[y,x] = map[y,x];
			}
		}
		this.size = size;
	}

	public IEnumerable<Vector3> Adjacent(Vector3 v) {
		int x = (int) v.x, y = (int) v.z;
		if (x < 0 || y < 0 || x >= size._2 || y >= size._1) {
			throw new ArgumentException("Vertex does no exist");
		}

		List<Vector3> adj = new List<Vector3>();
		foreach (Vector3 w in n4) {
			Vector3 p = v+w;
			x = (int) p.x;
			y = (int) p.z;
			if (x >= 0 && x < size._2 && y >= 0 && y < size._1 && map[y,x]) {
				adj.Add(p);
			}
		}
		return adj;
	}

	public float Cost(Vector3 v, Vector3 w) {
		if (v.x < 0 || v.z < 0 || v.x >= size._2 || v.z >= size._1 ||
			w.x < 0 || w.z < 0 || w.x >= size._2 || w.z >= size._1) {
			throw new ArgumentException("Vertex does no exist");
		}
		int dist = (int) (v-w).magnitude;
		if (dist > 1) {
			return float.MaxValue;
		}
		return dist;
	}

	private static readonly Vector3[] n4 = new Vector3[] {
		Vector3.zero,
		new Vector3( 1, 0,  0),
		new Vector3( 0, 0,  1),
		new Vector3(-1, 0,  0),
		new Vector3( 0, 0, -1)
	};
}
