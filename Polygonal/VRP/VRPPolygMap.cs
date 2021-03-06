﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class VRPPolygMap : AbstractPolygonMap {

	// Number of customers
	public int M { get; protected set; }

	// Customers positions
	public Vector3[] customers { get; protected set; }


	/*	First line N, number of vehicles
		next N lines, start positions of vehicles x y
		M, number of customers
		next M lines, customer positions x y
		count, number of vertices
		next count lines, polygon vertex description, x y button
	*/
	public VRPPolygMap(string filename) {
		StreamReader sr = new StreamReader(filename);
		try {
			// Vehicles
			string sN = sr.ReadLine();
			this.N = int.Parse(sN);
			this.starts = new Vector3[N];
			for (int i = 0; i < N; i++) {
				string[] sxy = sr.ReadLine().Split(' ');
				starts[i] = new Vector3(float.Parse(sxy[0]), 0, float.Parse(sxy[1]));
			}
			
			// Customers
			string sM = sr.ReadLine();
			this.M = int.Parse(sM);
			this.customers = new Vector3[M];
			for (int i = 0; i < M; i++) {
				string[] cxy = sr.ReadLine().Split(' ');
				customers[i] = new Vector3(float.Parse(cxy[0]), 0, float.Parse(cxy[1]));
			}

			// Polygons
			int count = int.Parse(sr.ReadLine());
			int[] button = new int[count];
			Vector3[] vertices = new Vector3[count];
			
			// Read vertices and buttons
			for (int i = 0; i < count; i++) {
				string[] line = sr.ReadLine().Split(' ');
				vertices[i] = new Vector3(float.Parse(line[0]), 0, float.Parse(line[1]));
				button[i] = int.Parse(line[2]);
			}

			// Initialize polygon collection
			this.polys = new List<Polygon>();
			List<Vector3> buffer = new List<Vector3>();
			for (int i = 0; i < count; i++) {
				buffer.Add(vertices[i]);
				if (button[i] == 3) {
					Polygon newPol = new Polygon(buffer);
					polys.Add(newPol);
					buffer.Clear();
				}
			}

		} finally {
			sr.Close();		// Close stream
		}
	}

	// Returns a list of customers
	public List<Vector3> GetCustomerPositions() {
		return new List<Vector3>(customers);
	}
}
