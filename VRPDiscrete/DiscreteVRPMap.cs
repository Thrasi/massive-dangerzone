using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DiscreteVRPMap : AbstractDiscreteMap {

	// Number of customers
	public int M { get; protected set; }

	// Customers
	public Vector3[] customers { get; protected set; }


	/* 	Reads the file for discrete map, the file should have this format:
		first line -> N -> number of vehicles
		next N lines -> Xs Ys -> start coordinates i-th of vehicle
		line int M -> number of customers
		next M lines x y customer coordinates
		R C -> number of rows and columns in a map (size of the map)
		next R lines with C times 1/0
	*/
	public DiscreteVRPMap(string filename) {
		StreamReader sr = new StreamReader(filename);
		try {
			// Number of vehicles
			string sN = sr.ReadLine();
			this.N = int.Parse(sN);
			this.starts = new Vector3[N];

			// Read coordinates for all vehicles
			for (int i = 0; i < N; i++) {
				string[] coords = sr.ReadLine().Split(' ');
				starts[i] = new Vector3(int.Parse(coords[0]), 0, int.Parse(coords[1]));
			}

			// Number of customers
			string sM = sr.ReadLine();
			this.M = int.Parse(sM);
			this.customers = new Vector3[M];
			for (int i = 0; i < M; i++) {
				string[] coords = sr.ReadLine().Split(' ');
				customers[i] = new Vector3(int.Parse(coords[0]), 0, int.Parse(coords[1]));
			}

			// Size of the map
			string[] sSize = sr.ReadLine().Split(' ');
			this.size = Tuple.Create(int.Parse(sSize[0]), int.Parse(sSize[1]));
			this.map = new bool[size._1, size._2];	// x and y are here inverted
			
			// Read the map
			for (int y = 0; y < size._1; y++) {
				string[] line = sr.ReadLine().Split(' ');
				for (int x = 0; x < size._2; x++) {
					map[y,x] = int.Parse(line[x]) == 0;
				}
			}
		} finally {
			sr.Close();
		}
	}

	// Returns a list of customers positions
	public List<Vector3> GetCustomerPositions() {
		return new List<Vector3>(customers);
	}

}
