using UnityEngine;
using System.Collections;

public class KinematicCarVehicle : DirectedVehicle<KinematicCarVehicle> {

	public static float maxPhi;
	public static float maxSpeed;
	private float L;

	public KinematicCarVehicle(KinematicCarVehicle parent, GameObject gobj,
		Vector3 relativePosition, float L) : base(parent, gobj, relativePosition) {

		this.L = L;
	}

	public override void Move(float dt) {

	}
}
