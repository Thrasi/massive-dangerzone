using UnityEngine;
using System.Collections.Generic;

namespace MechanicsUtility {

	/**
		Class that has all needed functions for motion mechanics.
	**/
	public static class Mechanics {

		// Slowing distance cant be less than this
		private const float SLOWING_MIN = 3;

		// Taking current position and velocity and goal position, calculates
		// steering (acceleraton) needed to reach goal. Returned vector is normalized
		public static Vector3 Steer(Vector3 position, Vector3 velocity, Vector3 goal) {
			Vector3 desired = goal - position;
			return (desired - velocity).normalized;
		}

		// Same as above but returns not normalized steer
		public static Vector3 SteerMax(Vector3 position, Vector3 velocity, Vector3 goal) {
			Vector3 desired = goal - position;
			return desired - velocity;
		}

		// Acceleration in time step used for maximum braking
		public static Vector3 Brake(Vector3 velocity, float maxAcc, float dt) {
			Vector3 direction = -velocity.normalized;
			return direction * Mathf.Min(velocity.magnitude, maxAcc*dt);
		}

		// Stopping distance
		public static float StoppingDistance(float speed, float maxAcc) {
			return 0.5f * speed * speed / maxAcc;
		}

		// Calculates how much it needs to steer to reach the goal without considering
		// any obstacles and other vehicles
		public static Vector3 Steer(Vector3 oldVelocity,
			Vector3 pos, Vector3 goal, float maxAcc, float dt) {
			
			float dist = Vector3.Distance(pos, goal);
			float speed = oldVelocity.magnitude;
			float slowing = Mathf.Max(StoppingDistance(speed, maxAcc), SLOWING_MIN);
			Vector3 desired =  (goal - pos).normalized * (speed + maxAcc*dt);
			if (dist < slowing) {
				//desired /= slowing;
				desired = (goal-pos);	// Works for now
			}
			Vector3 steer = desired - oldVelocity;
			return Mathf.Min(maxAcc*dt, steer.magnitude) * steer.normalized;
		}

		// Although this function is very similar to Steer, I still prefer having both
		// because they have small differences and this one might need tweaks in future
		// This one returns preferred velocity of an agent, that includes stopping in
		// the end with precision
		public static Vector3 PrefVelocity(Vector3 oldVelocity,
			Vector3 pos, Vector3 goal, float maxAcc, float dt) {
			
			float dist = Vector3.Distance(pos, goal);
			if (dist < SLOWING_MIN) {
				return (goal - pos) / 2;
			}
			float speed = oldVelocity.magnitude;
			float slowing = Mathf.Max(StoppingDistance(speed, maxAcc), SLOWING_MIN);
			Vector3 desired =  (goal - pos).normalized * (speed + maxAcc*dt);
			if (dist < slowing) {
				desired /= slowing;
			}
			return desired;
		}

	}//End class

}//End namespace
