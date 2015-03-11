using UnityEngine;
using System.Collections.Generic;
using RVO;

public class DynamicRVO {

	public static DynamicRVO Instance = new DynamicRVO();

	private Simulator sim = Simulator.Instance;
	
	private const float maxSpeed = 10000;

	private List<Vector3> velocities = new List<Vector3>();
	private Vector3 vel;

	private DynamicRVO() {
	}

	public void setAgentDefaults(float neighborDist, int maxNeighbors,
            float timeHorizon, float timeHorizonObst, float R,
            Vector3 velocity) {
		
		sim.setAgentDefaults(neighborDist, maxNeighbors, timeHorizon, timeHorizonObst,
			R, maxSpeed, ToVec2(velocity));
		vel = velocity;
	}

	public void addAgent(Vector3 position) {
		sim.addAgent(ToVec2(position));
		velocities.Add(vel);
	}

	private float dt;
	public void setTimeStep(float dt) {
		this.dt = dt;
		sim.setTimeStep(dt);

	}

	public Vector3 getAgentPosition(int i) {
		return ToVec3(sim.getAgentPosition(i));
	}

	public void setAgentPrefVelocity(int i, Vector3 velocity) {
		sim.setAgentPrefVelocity(i, ToVec2(velocity));
	}

	public Vector3 getAgentVelocity(int i) {
		return velocities[i];
	}

	public void setAgentPosition(int i, Vector3 position) {
		sim.agents_[i].position_ = ToVec2(position);
	}

	private float maxAcc;
	public void setMaxAcceleration(float maxAcc) {
		this.maxAcc = maxAcc;
	}

	public void doStep() {
		int N = sim.getNumAgents();
		Vector3[] ppos = new Vector3[N];
		Vector3[] cvelocities = new Vector3[N];
		for (int i = 0; i < N; i++) {
			ppos[i] = getAgentPosition(i);
			cvelocities[i] = getAgentVelocity(i);
		}
		sim.doStep();
		for (int i = 0; i < N; i++) {
			Vector3 currPos = getAgentPosition(i);
			
			Vector3 desiredVelocity = ToVec3(sim.getAgentVelocity(i));
			Vector3 currentVelocity = cvelocities[i];
			Vector3 steer = desiredVelocity - currentVelocity;
			//if (Random.value < 0.1f && Vector3.Distance(currPos, goals[i]) > 10) {
			//	steer = Random.onUnitSphere;
			//	steer.y = 0;
			//}
			velocities[i] += steer.normalized * Mathf.Min(maxAcc*dt, steer.magnitude);
			//vehicles[i].transform.Translate(velocities[i]*dt, Space.World);
			setAgentPosition(i, ppos[i] + velocities[i]*dt);//vehicles[i].transform.position);
			//setAgentVelocity(i, velocities[i]);
		}
	}

	public void setAgentVelocity(int i, Vector3 velocity) {
		velocities[i] = velocity;
	}

	private static Vector3 ToVec3(RVO.Vector2 v) {
		return new Vector3(v.x(), 0, v.y());
	}

	private static RVO.Vector2 ToVec2(Vector3 v) {
		return new RVO.Vector2(v.x, v.z);
	}
}
