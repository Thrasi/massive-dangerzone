using UnityEngine;
using System.Collections;

public class KinematicPointVirtualStructure : KinematicPointLeaderFollower {

	// Just some formation
	protected override void InitFormation() {
		InitLeader();
		vehicles[0].gobj.renderer.enabled = false;

		AddVehicle(0, Vector3.back*10);
		AddVehicle(0, Vector3.forward*10);
		AddVehicle(0, Vector3.left*10);
		AddVehicle(0, Vector3.right*10);
		AddVehicle(0, backLeft*10);
		AddVehicle(0, backRight*10);
	}

	void OnDrawGizmos() {
		VirtualStructureGizmosDraw.OnDrawGizmos(vehicles);
	}
}
