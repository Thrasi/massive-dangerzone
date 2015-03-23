using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRP  {
	public float[,] distances;
	public int N, V;

	public VRP(int N, int V, float[,] dists) {
		this.N = N;
		this.V = V;
		distances = dists;
	}
}
