using UnityEngine;
using System.Collections;

public class KinematicCarVirtualStructure : KinematicCarLeaderFollower {

	// Initializes formation which has all the same parent
	protected override void InitFormation() {
		InitLeader();
		GameObject leaderVehicle = leader.gobj;
		leaderVehicle.transform.Find("CC_ME_Body_R4").renderer.enabled = false;
		leaderVehicle.transform.Find("CC_ME_Wheel_BL").renderer.enabled = false;
		leaderVehicle.transform.Find("CC_ME_Wheel_BR").renderer.enabled = false;
		leaderVehicle.transform.Find("CC_ME_Wheel_FL").renderer.enabled = false;
		leaderVehicle.transform.Find("CC_ME_Wheel_FR").renderer.enabled = false;
		Vector3 pos = followCamera.transform.position;
		pos.y += 10;
		pos.z -= 10;
		followCamera.transform.position = pos;

		AddVehicle(0, Vector3.back*10);
		AddVehicle(0, Vector3.left*10);
		AddVehicle(0, Vector3.right*10);
		AddVehicle(0, backLeft*20);
		AddVehicle(0, backRight*20);
		AddVehicle(0, frontLeft30*20);
		AddVehicle(0, frontRight30*20);
		AddVehicle(0, frontLeft75*20);
		AddVehicle(0, frontRight75*20);
	}

	void OnDrawGizmos() {
		VirtualStructureGizmosDraw.OnDrawGizmos(vehicles);
	}
}
