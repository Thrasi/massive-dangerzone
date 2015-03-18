using UnityEngine;
using System.Collections.Generic;

public class KinematicPointVehicle : Vehicle<KinematicPointVehicle> {

	// Free parameter, maximum speed, write-only
	public static float maxSpeed { private get; set; }
	
	// Using relative position computes end position
	public override Vector3 endPosition {
		get {
			if (parent == null) {
				return position;
			}
			return parent.position + relativePosition;
		}
	}

	// Constructor
	public KinematicPointVehicle(GameObject vehicle, KinematicPointVehicle parent,
		Vector3 relativePosition) : base(parent, vehicle, relativePosition) {
	}

	// Moves kinematicly in endPosition according to its parent.
	// If this is the leader, it doesnt move, it needs push from outside.
	public override void Move(float dt) {
		if (parent == null) {
			return;
		}
		Vector3 toMove = endPosition - position;
		Translate(toMove.normalized * Mathf.Min(maxSpeed*dt, toMove.magnitude));
	}
}
