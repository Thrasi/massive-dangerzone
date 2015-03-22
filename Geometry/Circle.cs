using UnityEngine;
using System.Collections;

public class Circle {

	// Radius
	public readonly float r;

	// Center coordinates
	public readonly Vector3 center;


	// Usual circle constructor
	public Circle(float r, Vector3 center) {
		this.r = r;
		this.center = center;
	}

	// Constructor with tangential vector, radius and point on circle
	public Circle(float r, Vector3 point, Vector3 tangent, bool centerOnLeft) {
		float turnAngle = centerOnLeft ? -90 : 90;
		Vector3 toCenter = Quaternion.Euler(0, turnAngle, 0) * tangent.normalized;
		this.r = r;
		this.center = point + r * toCenter;
	}

	// Checks if point is inside circle
	public bool IsInside(Vector3 point) {
		return Vector3.Distance(point, center) < r;
	}

	// Finds closest point on circle
	public Vector3 ClosestOn(Vector3 point) {
		Vector3 toPoint = (point - center).normalized;
		return center + toPoint * r;
	}

}
