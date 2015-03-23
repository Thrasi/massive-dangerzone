using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Geometry;

public abstract class LocalMoveVehicle {

	// Neighborhood size
	public static int N = 3;

	// Gameobject for the vehicle
	protected GameObject gobj;

	// Position of the vehicle
	public Vector3 position { get { return gobj.transform.position; } }


	// Resulting move
	public Vector3 resultMove { get; private set; }

	// All moves that sum up to resulting move
	public List<Vector3> moves { get; private set; }


	// This vehicles neighbors
	private IEnumerable<Tuple<Vector3, float>> nClosest;
	public IEnumerable<Tuple<Vector3, float>> closest3 {
		get { return nClosest.Take(Mathf.Min(N, 3)); }
	}
	public IEnumerable<Tuple<Vector3, float>> fartherThan3 {
		get { return nClosest.Skip(Mathf.Min(N, 3)); }
	}

	
	// Constructor
	public LocalMoveVehicle(GameObject gobj) {
		this.resultMove = Vector3.zero;
		this.moves = new List<Vector3>();
		this.gobj = gobj;
	}

	// How is vehicle supposed to move given other vehicles
	// Parameter toMove decides if the vehicle should really be moved, if it is
	// set to false, then only parameters will be updates, its done this way
	// so that gizmos drawing is easier
	public void Move(IEnumerable<LocalMoveVehicle> vehicles,
		float distance, float dt, bool toMove) {

		IEnumerable<Vector3> positions =
				from veh in vehicles
				where veh != this
				select veh.position;
		
		moves.Clear();
		resultMove = Vector3.zero;
		nClosest = from pd in positions.Closest(position, N) orderby pd._2 select pd;
		
		// Finding the best move
		foreach (Tuple<Vector3, float> pd in closest3) {
			Vector3 pointPosition = pd._1;
			float distToPoint = pd._2;
			Vector3 move;

			// For the closest 3, use stronger forces
			if (distToPoint < distance) {		// Closer than needed
				move = (position - pointPosition).normalized * (distance - distToPoint);
			} else {							// Further away than needed
				move = (pointPosition - position).normalized * (distToPoint - distance);
			}

			moves.Add(move);
			resultMove += move;
		}

		// If there is something wrong with large neighborhood, fix this
		foreach (Tuple<Vector3, float> pd in fartherThan3) {
			Vector3 pointPosition = pd._1;
			float distToPoint = pd._2;

			// For the far points, use weak forces to slowly bring them together
			if (distToPoint > distance) {
				resultMove += (pointPosition - position).normalized * 0.1f;
				/// (distToPoint * distToPoint);
			}
		}

		if (toMove) {
			VehicleMove(dt);
		}
	}

	// Specific vehicle move
	protected abstract void VehicleMove(float dt);

	// Translate adapter
	public void Translate(Vector3 move) {
		gobj.transform.Translate(move, Space.World);
	}

	// Rotate adapter
	public void Rotate(float angle) {
		gobj.transform.Rotate(0, angle, 0, Space.World);
	}

	// For debugging purposes
	protected void print(object s) {
		Debug.Log(s);
	}

}
