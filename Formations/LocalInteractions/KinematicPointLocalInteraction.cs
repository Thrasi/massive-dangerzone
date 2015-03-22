using UnityEngine;
using System.Collections;

public class KinematicPointLocalInteraction : LocalInteraction<KinematicPointLMVehicle> {

	// Speed of the point
	public float speed = 1;


	// Move the leader
	protected override void MoveLeader(float dt) {
		Vector3 move = LocalInteractionUtilities.DecidePointMove().normalized;
		leader.Translate(move * speed * dt);
	}

	// Update speed
	protected override void UpdateParameters() {
		KinematicPointLMVehicle.speed = speed;
	}

	// Loads kinematic point vehicle
	protected override void LoadGameObject() {
		vehicleObject = Resources.Load("GameObjects/SphericalVehicle") as GameObject;
		vehicleObject.transform.localScale = Vector3.one;
	}

	// Just instantiates new vehicle
	protected override KinematicPointLMVehicle CreateVehicle(GameObject veh) {
		return new KinematicPointLMVehicle(veh);
	}
}
