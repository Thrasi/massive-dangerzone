using UnityEngine;
using System.Collections;

public class DynamicPointVirtualStructure : DynamicPointLeaderFollower {

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
		AddVehicle(0, forwardLeft*10);
		AddVehicle(0, forwardRight*10);
		AddVehicle(0, Vector3.right*20);
		AddVehicle(0, Vector3.left*20);
		AddVehicle(0, Vector3.back*20);
		AddVehicle(0, Vector3.forward*20);
		AddVehicle(0, backRight30*15);
		AddVehicle(0, backLeft30*15);
		AddVehicle(0, frontLeft30*15);
		AddVehicle(0, frontRight30*15);
	}

	void OnDrawGizmos() {
		VirtualStructureGizmosDraw.OnDrawGizmos(vehicles);
	}
}
