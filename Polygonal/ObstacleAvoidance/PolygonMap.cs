using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PolygonMap : AbstractPolygonMap {

	// Goal positions
	public Vector3[] goals { get; protected set; }


	/*	First line N, number of vehicles
		next N lines, start and goal positions of vehicles sx sy gx gy
		count, number of vertices
		next count lines, polygon vertex description, x y button
	*/
	public PolygonMap(string filename) {
		StreamReader sr = new StreamReader(filename);
		try {
			// Vehicles
			string sN = sr.ReadLine();
			this.N = int.Parse(sN);
			this.starts = new Vector3[N];
			this.goals = new Vector3[N];
			for (int i = 0; i < N; i++) {
				string[] sxy = sr.ReadLine().Split(' ');
				starts[i] = new Vector3(float.Parse(sxy[0]), 0, float.Parse(sxy[1]));
				goals[i] = new Vector3(float.Parse(sxy[2]), 0, float.Parse(sxy[3]));
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
			List<Vector2> buffer = new List<Vector2>();
			for (int i = 0; i < count; i++) {
				buffer.Add(new Vector2(vertices[i].x, vertices[i].z));
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

}
