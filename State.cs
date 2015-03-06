using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*
	Class describes state in the scene from AStar algorithn.
*/
public class State {

	// Positions of all vehicles
	public Vector3 pos { get; private set; }

	// Time instance
	public int t { get; private set; }


	// Set time and position
	public State(Vector3 pos, int t) {
		this.pos = pos;
		this.t = t;
	}

	// Both position and time are equal
	public override bool Equals(object other) {
		if (!(other is State)) {
			return false;
		}
		State obj = other as State;
		return this.pos.Equals(obj.pos) && this.t == obj.t;
	}

	// For dictionary
	public override int GetHashCode() {
		return pos.GetHashCode() + 31 * t;
	}

	// For debugging
	public override string ToString() {
		return string.Format("({0}, {1})", pos, t);
	}
}
