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

		// Finds closest n points
		public static IEnumerable<Tuple<Vector3, float>> Closest(
			this IEnumerable<Vector3> positions, Vector3 point, int n) {
			
			// Find closest
			FFFBHeap<Vector3> heap = new FFFBHeap<Vector3>(n);
			foreach (Vector3 p in positions) {
				heap.Insert(Vector3.Distance(p, point), p);
			}

			// Turn them into tuples
			List<Tuple<Vector3, float>> closest = new List<Tuple<Vector3, float>>();
			foreach (KeyValuePair<Vector3, float> kv in heap) {
				closest.Add(Tuple.Create(kv.Key, kv.Value));
			}
			return closest;
		}
	}

}