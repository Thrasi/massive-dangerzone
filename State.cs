using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*
	Class describes state in the scene from AStar algorithn.
*/
public class State {

	// Positions of all vehicles
	public Vector3[] positions { get; private set; }

	// Copy of the enumerable
	public State(IEnumerable<Vector3> positions) {
		this.positions = new List<Vector3>(positions).ToArray();
	}

	// If all positions are equal
	public override bool Equals(object other) {
		if (!(other is State)) {
			return false;
		}
		State obj = other as State;
		return Enumerable.SequenceEqual(this.positions, obj.positions);
	}

	// For dictionary
	public override int GetHashCode() {
		int s = 0;
		foreach (Vector3 v in positions) {
			s += 17 * (v.x.GetHashCode() + 31 * v.z.GetHashCode());
		}
		return s;
	}

	// For debugging
	public override string ToString() {
		return string.Join(" ", positions.Select(x => x.ToString()).ToArray());
	}
}
