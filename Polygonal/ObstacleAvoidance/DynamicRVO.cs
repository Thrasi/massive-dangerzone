using UnityEngine;
using System.Collections;
using RVO;

public class DynamicRVO {

	public static DynamicRVO Instance = new DynamicRVO();

	private Simulator sim = Simulator.Instance;
	
	private const float maxSpeed = 10000;

	private DynamicRVO() {
	}

	public void setAgentDefaults(float neighborDist, int maxNeighbors,
            float timeHorizon, float timeHorizonObst, float R,
            Vector3 velocity) {
		
		sim.setAgentDefaults(neighborDist, maxNeighbors, timeHorizon, timeHorizonObst,
			R, maxSpeed, ToVec2(velocity));
	}

	private static Vector3 ToVec3(RVO.Vector2 v) {
		return new Vector3(v.x(), 0, v.y());
	}

	private static RVO.Vector2 ToVec2(Vector3 v) {
		return new RVO.Vector2(v.x, v.z);
	}
}
