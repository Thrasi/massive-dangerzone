﻿using UnityEngine;
using System.Collections;
using System;

/**
	Class describes edges or in other words line segments. Includes
	methods which are used for graph construction.
*/
public class Edge {

	// Const to ensure that the algorithms finds proper intersections
	private const float eps = 0.0001f;

	// Const for angle of point being on the edge
	private const float angEps = 0.1f;

	// Two vertices that define the edge
	public Vector3 v { get; private set; }
	public Vector3 w { get; private set; }


	// Constructor
	public Edge(Vector3 v, Vector3 w) {
		this.v = v;
		this.w = w;
	}

	// Returns length of the edge
	public float Length() {
		return Vector3.Distance(v, w);
	}

	// Checks if two edges intersect
	// Edges do not intersect if they lie on one another
	// or if they have a common vertex
	public bool Intersect(Edge other) {
		// Checking for common vertex
		if (	this.v.Equals(other.v) || this.v.Equals(other.w)
			|| 	this.w.Equals(other.v) || this.w.Equals(other.w)) {
			
			return false;
		}

		// Create lines
		Line l1 = new Line(this);
		Line l2 = new Line(other);

		// Find intersect
		Vector3? tmp = l1.Intersection(l2);
		if (!tmp.HasValue) {	// No intersection
			return false;
		}
		Vector3 p = tmp.Value;

		// Making this for easier last return
		Vector3 v1 = this.v;
		Vector3 w1 = this.w;
		Vector3 v2 = other.v;
		Vector3 w2 = other.w;

		float pxPe = p.x + eps;
		float pxMe = p.x - eps;
		float pyPe = p.z + eps;
		float pyMe = p.z - eps;

		// Complicated last return
		return 	pxPe >= Mathf.Min(v1.x, w1.x) && pxMe <= Mathf.Max(v1.x, w1.x)
			&&	pxPe >= Mathf.Min(v2.x, w2.x) && pxMe <= Mathf.Max(v2.x, w2.x)
			&&	pyPe >= Mathf.Min(v1.z, w1.z) && pyMe <= Mathf.Max(v1.z, w1.z)
			&&	pyPe >= Mathf.Min(v2.z, w2.z) && pyMe <= Mathf.Max(v2.z, w2.z);
	}

	// Finds intersecting point of 2 edges, returns null if they dont intersect
	public Vector3? Intersection(Edge other) {
		// Create lines
		Line l1 = new Line(this);
		Line l2 = new Line(other);

		// Find intersect
		Vector3? tmp = l1.Intersection(l2);
		if (!tmp.HasValue) {	// No intersection
			return null;
		}
		Vector3 p = tmp.Value;

		// Point must be on both edges
		if (this.IsOn(p) && other.IsOn(p)) {
			return p;
		}
		return null;
	}

	// Checks whether a point is on the egde
	private bool IsOn(Vector3 point) {
		float len = Length();
		return Vector3.Angle(point-v, w-v) < angEps
			&& Vector3.Angle(point-w, v-w) < angEps
			&& Vector3.Distance(point, v) <= len
			&& Vector3.Distance(point, w) <= len;
	}

	// Distance from point to edge
	public float Distance(Vector3 point) {
		float a = Vector3.Distance(point, v);
		float b = Vector3.Distance(point, w);
		float c = Vector3.Distance(v, w);
		float aa = a*a, bb = b*b, cc = c*c;
		if (aa + cc > bb && bb + cc > aa) {		// Acute
			return new Line(this).Distance(point);
		}
		return Mathf.Min(a, b);
	}

	// Returns vector that is vertical to this edge and directed towards the edge
	// Returned vector is normalized
	public Vector3 Vertical(Vector3 point) {
		Vector3 edgeVec = w - v;
		Vector3 pointVec = point - v;
		float dir = Vector3.Cross(edgeVec, pointVec).y;
		if (dir < 0) {
			return (Quaternion.Euler(0, 90, 0) * edgeVec).normalized;
		}
		return (Quaternion.Euler(0, -90, 0) * edgeVec).normalized;
	}

	// Draws the edge using Gizmos
	public void GizmosDraw(Color color) {
		Color tmp = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(
			new Vector3(v.x, 0.0f, v.z),
			new Vector3(w.x, 0.0f, w.z)
		);
		Gizmos.color = tmp;
	}

	// Two edges are equal if they have equal vertices
	override public bool Equals(object other) {
		if (!(other is Edge)) {
			return false;
		}
		Edge o = other as Edge;
		return 	(o.v.Equals(this.v) && o.w.Equals(this.w))
			||	(o.v.Equals(this.w) && o.w.Equals(this.v));
	}

	// To use in dictionaries
	override public int GetHashCode() {
		return v.GetHashCode() + w.GetHashCode();
	}

	// For debugging
	override public string ToString() {
		return "[ " + v.ToString() + ", " + w.ToString() + " ]";
	}

}
