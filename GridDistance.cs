using UnityEngine;
using System.Collections.Generic;
using System;

public class GridDistance {

	private Tuple<int, int> size;
	public int[,] dist;
	private DiscreteGraph gph;
	private Vector3 pos;

	public GridDistance(DiscreteGraph gph, Vector3 pos) {
		this.size = gph.size;
		if (pos.x < 0 || pos.z < 0 || pos.x >= size._2 || pos.z >= size._1) {
			throw new ArgumentException("Vertex does no exist");
		}
		this.gph = gph;
		this.pos = pos;
		this.dist = new int[size._1, size._2];
		for (int y = 0; y < size._1; y++) {
			for (int x = 0; x < size._2; x++) {
				dist[y,x] = int.MaxValue;
			}
		}
		bfs();
	}

	private void bfs() {
		dist[(int) pos.z, (int) pos.x] = 0;
		HashSet<Vector3> visited = new HashSet<Vector3>();
		visited.Add(pos);
		LinkedList<Vector3> open = new LinkedList<Vector3>();
		open.AddLast(pos);

		while (open.Count > 0) {
			Vector3 curr = open.First.Value;
			open.RemoveFirst();
			int x = (int) curr.x;
			int y = (int) curr.z;

			foreach (Vector3 v in gph.Adjacent(curr)) {
				if (visited.Contains(v)) {
					continue;
				}
				open.AddLast(v);
				dist[(int) v.z, (int) v.x] = dist[y,x] + 1;
			}
		}
	}
}
