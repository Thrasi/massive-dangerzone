using UnityEngine;
using System.Collections.Generic;

public interface Graph {

	IEnumerable<Vector3> Adjacent(Vector3 v);
	float Cost(Vector3 v, Vector3 w);
}
