using UnityEngine;
using System.Collections.Generic;

/**
	Base class for all vehicles. They are supposed to implement how to move from
	point to point so they can reach desired positions in the formation.
**/
public abstract class Vehicle<T> where T : Vehicle<T> {

	// Parent in leader follower formation
	public T parent;

	// GameObject of this vehicle
	public readonly GameObject gobj;

	// Position of the vehicle
	public Vector3 position { get { return gobj.transform.position; } }

	// Relative position behind his leader
	public Vector3 relativePosition;

	// Somehow get endPosition
	public abstract Vector3 endPosition { get; }


	// Constructor
	public Vehicle(T parent, GameObject gobj, Vector3 relativePosition) {
		this.parent = parent;
		this.gobj = gobj;
		this.relativePosition = relativePosition;
	}

	// Translate adapter
	public void Translate(Vector3 move) {
		gobj.transform.Translate(move, Space.World);
	}

	// Rotate adapter
	public void Rotate(float angle) {
		gobj.transform.Rotate(0, angle, 0, Space.World);
	}

	// The most important part, moving the vehicle
	public abstract void Move(float dt);

}
