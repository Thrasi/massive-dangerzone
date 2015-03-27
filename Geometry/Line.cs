using UnityEngine;
using System.Collections;
using System;

// Unlike Edge class, this class describes line as infinite construction
public class Line {

	// line parameters as in ax + by + c = 0
	public readonly float a;
	public readonly float b;
	public readonly float c;

	// Constructs line from 2 points
	public Line(Vector3 v, Vector3 w) {
		if (v.x == w.x) {
			a = 1.0f;
			b = 0.0f;
			c = -v.x;
		} else {
			float k = (v.z - w.z) / (v.x - w.x);
			a = -k;
			b = 1.0f;
			c = k * v.x - v.z;
		}
	}

	// Construct line from an edge
	public Line(Edge e) : this(e.v, e.w) {
	}

	// Returns slope of the line
	public float Slope() {
		return a/b;
	}

	// Returns distance from this line to given point
	public float Distance(Vector3 point) {
		float x = point.x, y = point.z;
		return Mathf.Abs(a*x + b*y + c) / Mathf.Sqrt(a*a + b*b);
	}

	// Finds intersection between this and other line
	// Returns null if matrix is singular (lines are parallel)
	public Vector3? Intersection(Line other) {
		float det = Det2(this.a, this.b, other.a, other.b);
		if (Mathf.Abs(det) < 0.0001f) {		// Singular matrix, no intersection
			return null;
		}

		float detX = Det2(-this.c, this.b, -other.c, other.b);
		float detY = Det2(this.a, -this.c, other.a, -other.c);
		return new Vector3(detX/det, 0, detY/det);
	}

	// Computes 2-by-2 determinant
	//  a b
	//  c d
	private float Det2(float a, float b, float c, float d) {
		return a * d - b * c;
	}
}