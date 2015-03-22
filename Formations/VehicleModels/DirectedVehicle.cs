using UnityEngine;
using System.Collections.Generic;

public abstract class DirectedVehicle<T> : Vehicle<T> where T : DirectedVehicle<T> {

	// Direction of the vehicle
	public virtual Vector3 direction { get { return gobj.transform.forward; } }
	
	// End position of vehicle
	public override Vector3 endPosition {
		get {
			if (parent == null) {
				return position;
			}
			Vector3 relPos = Quaternion.Euler(0, parent.angle, 0) * relativePosition;
			return parent.position + relPos;
		}
	}

	// How much is vehicle tilted from z axis
	protected float angle {
		get { return Tangents.RotationAngle(Vector3.forward, direction); }
	}

	// How much is vehicle rotated from vector to end position
	protected float angleToEnd {
		get { return Tangents.RotationAngle(direction, endPosition - position); }
	}

	// Difference between rotation angle of this vehicle and its parent
	protected float angleToParent {
		get {
			return parent == null ? 0 :
				Tangents.RotationAngle(direction, parent.direction);
		}
	}


	// Constructor
	public DirectedVehicle(T parent, GameObject gobj,
		Vector3 relativePosition) : base(parent, gobj, relativePosition) {
	}
}
