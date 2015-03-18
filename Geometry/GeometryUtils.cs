using UnityEngine;
using System.Collections.Generic;

namespace Geometry {

	public static class GeometryUtils {

		// Creates hexagon which 2 sides are parallel with x axis
		public static List<Edge> Hexagon(Vector3 center, float R) {
			Vector3 point = center + Quaternion.Euler(0, -30, 0) * Vector3.forward * R;
			Vector3 direction = Vector3.right;
			List<Edge> edges = new List<Edge>();
			for (int i = 0; i < 6; i++) {
				edges.Add(new Edge(point, point + direction*R));
				point += direction*R;
				direction = Quaternion.Euler(0, 60, 0) * direction;
			}
			return edges;
		}

		// Convert angle to radians
		public static float ToRad(this float angle) {
			return angle * Mathf.PI / 180;
		}

		// Convert angle to degrees
		public static float ToDeg(this float angle) {
			return angle * 180 / Mathf.PI;
		}
	}

}