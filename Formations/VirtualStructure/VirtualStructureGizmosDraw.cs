using UnityEngine;
using System.Collections.Generic;
using Geometry;

public static class VirtualStructureGizmosDraw {

	// Draw Gizmos for virtual structures
	public static void OnDrawGizmos<T>(List<T> vehicles) where T : Vehicle<T> {
		Gizmos.color = Color.blue;
		foreach (T v in vehicles) {
			// Draw positions
			foreach (Edge e in GeometryUtils.Hexagon(v.endPosition, 0.7f)) {
				e.GizmosDraw(Color.blue);
			}
		}

		if (vehicles.Count > 0) {
			Gizmos.DrawSphere(vehicles[0].position, 0.5f);
		}
	}
}
