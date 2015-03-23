using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DiscreteVRPMap : AbstractDiscreteMap {

	// Number of customers
	public int M { get; protected set; }

	// Customers
	public Vector3[] customers { get; protected set; }
	public FloydMarshall g;
	public VRP vrp;


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

		// create the graph
		g = new FloydMarshall ();
		for (int y=0;y<size._1;y++){
			for (int x=0;x<size._2;x++){
				if(map[y,x]) {
					VertexG vertex = new VertexG(new Vector3(x,0,y));
//					Debug.Log(vertex);
					g.Add(vertex);
					
					VertexG destination;
					int kk=0;
					if (y-1>=0) {
						if(map[y-1,x]) {
							destination = new VertexG(new Vector3(x,0,y-1));
							g.AddEdge (vertex, destination, 1);
							kk++;
						}
					}
					if (y+1<size._1) {
						if(map[y+1,x]) {
							destination = new VertexG(new Vector3(x,0,y+1));
							g.AddEdge (vertex, destination, 1);
							kk++;
						}
					}
					if (x-1>=0) {
						if(map[y,x-1]) {
							destination = new VertexG(new Vector3(x-1,0,y));
							g.AddEdge (vertex, destination, 1);
							kk++;
						}
					}
					if (x+1<size._2) {
						if(map[y,x+1]) {
							destination = new VertexG(new Vector3(x+1,0,y));
							g.AddEdge (vertex, destination, 1);
							kk++;
						}
					}
//					Debug.Log("vertex: "+vertex+",added "+kk+" neighbours");
				}
			}
		}
		// calculate the distances between all vertices
		g.calcDists ();
//
		// Create the adjacency matrix used for VRP 
		int C = customers.Length;
		float [,] d = new float[N+C,N+C];
		VertexG v, u;
//		foreach (Vector3 h in customers) {
//			Debug.Log("Cust: "+ h);	
//		}
//		foreach (Vector3 h in starts) {
//			Debug.Log("Vehicle: "+ h);	
//		}
		for (int c=0; c<C; c++) { // customer-customer
			for (int cc=c; cc<C; cc++) {
				v = new VertexG(customers[c]);
				u = new VertexG(customers[cc]);
				float f = g.distance(v, u);
				d [c,cc] = f;
				d [cc,c] = f;
			}
		}
		
		for (int n=0; n<N; n++) { // vehicle-vehicle
			for (int nn=n; nn<N; nn++) {
				v = new VertexG(starts[n]);
				u = new VertexG(starts[nn]);
				float f = g.distance(v,u);
				d [C+n,C+nn] = f;
				d [C+nn,C+n] = f;
			}
		}
		
		for (int n=0; n<N; n++) { // vehicle-customer
			for (int c=0; c<C; c++) {
				v = new VertexG(starts[n]);
				u = new VertexG(customers[c]);
				float f = g.distance(v,u);
				d [C+n,c] = f;
				d [c,C+n] = f;
			}
		}
		
		vrp = new VRP(C,N,d);
	}

	// Returns a list of customers positions
	public List<Vector3> GetCustomerPositions() {
		return new List<Vector3>(customers);
	}

}
