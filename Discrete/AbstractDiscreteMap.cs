using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractDiscreteMap {

	// Number of vehicles
	public  int N { get; protected set; }

	// Size of the map, (y, x) == (rows, columns)
	public Tuple<int, int> size { get; protected set; }

	// Start coordinates of all vehicles
	public Vector3[] starts { get; protected set; }

	// Map of obstacles, true means free, false means obstacle
	protected bool[,] map;

	// Neighborhood to use
	protected Vector3[] neighborhood;


	// Set always 4-connected neighborhood
	protected AbstractDiscreteMap() {
		this.neighborhood = n4;
	}

	// Returns a list of positions for obstacles
	public List<Vector3> GetObstaclePositions() {
		List<Vector3> positions = new List<Vector3>();
		for (int y = 0; y < size._1; y++) {
			for (int x = 0; x < size._2; x++) {
				if (!map[y,x]) {
					positions.Add(new Vector3(x, 0, y));
				}
			}
		}
		return positions;
	}

	// Returns the list of start positions
	public List<Vector3> GetStartPositions() {
		return new List<Vector3>(starts);
	}


	// Here are all neighborhoods
	// Only n4 is used at the moment

	protected static readonly Vector3[] n4 = new Vector3[] {
		Vector3.zero,
		new Vector3( 1, 0,  0),
		new Vector3( 0, 0,  1),
		new Vector3(-1, 0,  0),
		new Vector3( 0, 0, -1)
	};

	// To suppress variable not used
	#pragma warning disable 0414

	protected static readonly Vector3[] n8 = new Vector3[] {
		Vector3.zero,
		new Vector3( 1, 0,  0),
		new Vector3( 0, 0,  1),
		new Vector3(-1, 0,  0),
		new Vector3( 0, 0, -1),
		new Vector3( 1, 0,  1),
		new Vector3(-1, 0,  1),
		new Vector3(-1, 0, -1),
		new Vector3( 1, 0, -1)
	};

	protected static readonly Vector3[] n16 = new Vector3[] {
		Vector3.zero,
		new Vector3( 1, 0,  0), new Vector3( 2, 0,  1),
		new Vector3( 0, 0,  1), new Vector3(-2, 0,  1),
		new Vector3(-1, 0,  0), new Vector3( 2, 0, -1),
		new Vector3( 0, 0, -1), new Vector3(-2, 0, -1),
		new Vector3( 1, 0,  1), new Vector3( 1, 0,  2),
		new Vector3(-1, 0,  1), new Vector3( 1, 0, -2),
		new Vector3(-1, 0, -1), new Vector3(-1, 0,  2),
		new Vector3( 1, 0, -1), new Vector3(-1, 0, -2)
	};	

	#pragma warning restore 0414
}
