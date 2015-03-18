using UnityEngine;
using System.Collections;

public class DiffDriveVirtualStructure : DiffDriveLeaderFollower {

	// Some formation
	protected override void InitFormation() {
		// Create leader
		vehicles.Add(
			new DiffDriveVehicle(
				Instantiate(vehicle, Vector3.zero, Quaternion.Euler(0,-90,0)) as GameObject,
				null,
				Vector3.zero
			)
		);
		followCamera.transform.parent = leader.gobj.transform;
		leader.gobj.transform.parent = transform;
		leader.gobj.transform.Find("Body").renderer.enabled = false;
		leader.gobj.transform.Find("Tower").renderer.enabled = false;

		// Add followers
		AddVehicle(0, Vector3.left * 10);
		AddVehicle(0, Vector3.right * 10);
		AddVehicle(0, Vector3.back * 10);
		AddVehicle(0, Vector3.left * 20);
		AddVehicle(0, Vector3.right * 20);
		AddVehicle(0, frontLeft30 * 15);
		AddVehicle(0, frontRight30 * 15);
		AddVehicle(0, backLeft30 * 15);
		AddVehicle(0, backRight30 * 15);
		AddVehicle(0, backLeft * 20);
		AddVehicle(0, backRight * 20);
	}

	void OnDrawGizmos() {
		VirtualStructureGizmosDraw.OnDrawGizmos(vehicles);
	}
}
