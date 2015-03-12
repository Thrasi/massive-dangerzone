using UnityEngine;
using System.Collections.Generic;
using RVO;

/**
	This is adapter class for dynamic usage of RVO library.
	It is not a true adapter, but slightly modified to fulfill my needs
	for dynamic point collision avoidance. Concrete, agent velocities after doing step
	is not as it is supposed to be.
**/
public class DynamicRVO {

	// Singleton instance
	public static DynamicRVO Instance = new DynamicRVO();

	// Adaptee
	private Simulator sim = Simulator.Instance;
	
	// Max speed, as big as possible but not inf
	private const float maxSpeed = 10000;

	// Vehicle velocities
	private List<Vector3> velocities = new List<Vector3>();
	
	// Agent default velocity
	private Vector3 defaultVelocity;

	// Time step used for simulation
	private float dt;

	// Max acceleration
	private float maxAcc;

	// Probability of generating random acceleration, used for breaking deadlocks
	private float probGenerate = 0.0f;

	// To generate random acceleration
	private bool generateRandAcc = false;


	// Part of singleton
	private DynamicRVO() {
	}

	// Sets default parameters
	public void setAgentDefaults(float neighborDist, int maxNeighbors,
            float timeHorizon, float timeHorizonObst, float R,
            Vector3 velocity) {
		
		sim.setAgentDefaults(neighborDist, maxNeighbors, timeHorizon, timeHorizonObst,
			R, maxSpeed, ToVec2(velocity));
		defaultVelocity = velocity;
	}

	// Adds agent
	public void addAgent(Vector3 position) {
		sim.addAgent(ToVec2(position));
		velocities.Add(defaultVelocity);
	}

	// Setting simulation time step
	public void setTimeStep(float dt) {
		this.dt = dt;
		sim.setTimeStep(dt);
	}

	// Returns position of ith agent
	public Vector3 getAgentPosition(int i) {
		return ToVec3(sim.getAgentPosition(i));
	}

	// Setter for agent position
	public void setAgentPosition(int i, Vector3 position) {
		sim.agents_[i].position_ = ToVec2(position);
	}

	// Getter for agent velocity
	public Vector3 getAgentVelocity(int i) {
		return velocities[i];
	}

	// Setter for velocity
	public void setAgentVelocity(int i, Vector3 velocity) {
		velocities[i] = velocity;
	}

	// Sets preferred velocity for ith agent
	public void setAgentPrefVelocity(int i, Vector3 velocity) {
		sim.setAgentPrefVelocity(i, ToVec2(velocity));
	}

	// Setter for max acceleration
	public void setMaxAcceleration(float maxAcc) {
		this.maxAcc = maxAcc;
	}

	// Setter for probability
	public void setAccelerationProbability(float prob) {
		this.probGenerate = prob;
	}

	// Setter for enabling generation of random accelerations
	public void setGenerate(bool gen) {
		this.generateRandAcc = gen;
	}

	// The most important method, does a dynamic step
	public void doStep() {
		// Compute current positions and velocities
		int N = sim.getNumAgents();
		Vector3[] currPos = new Vector3[N];
		Vector3[] currVel = new Vector3[N];
		for (int i = 0; i < N; i++) {
			currPos[i] = getAgentPosition(i);
			currVel[i] = getAgentVelocity(i);
		}

		// Do step
		sim.doStep();

		// Compute new positions
		for (int i = 0; i < N; i++) {
			Vector3 desiredVelocity = ToVec3(sim.getAgentVelocity(i));
			Vector3 currentVelocity = currVel[i];
			Vector3 steer = desiredVelocity - currentVelocity;
			
			// Generate random acceleration instead
			if (generateRandAcc && Random.value < probGenerate) {
				steer = Random.onUnitSphere;
				steer.y = 0;
				steer = steer.normalized;
			}
			
			// Set new velocities and positions
			velocities[i] += steer.normalized * Mathf.Min(maxAcc*dt, steer.magnitude);
			setAgentPosition(i, currPos[i] + velocities[i]*dt);
		}
	}
	
	// Convert rvo Vector2 to unity Vector3
	private static Vector3 ToVec3(RVO.Vector2 v) {
		return new Vector3(v.x(), 0, v.y());
	}

	// Convert unity Vector3 to rvo Vector2
	private static RVO.Vector2 ToVec2(Vector3 v) {
		return new RVO.Vector2(v.x, v.z);
	}
}
