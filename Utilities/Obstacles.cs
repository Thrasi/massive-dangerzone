using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ObstaclesExt {

	// Type alias for obstacles
	using Obst = IEnumerable<Polygon>;
	
	/**
		A bunch of extension methods to manipulate obstacles. Obstacles have type
		IEnumerable<Polygon>
	**/
	public static class Obstacles {

		// Returns all vertices in all obstacles
		public static List<Vector3> Vertices(this Obst polys) {
			List<Vector3> vert = new List<Vector3>();
			foreach (Polygon pol in polys) {
				vert.AddRange(pol.Vertices());
			}
			return vert;
		}

		// Same as Vertices but returns an IEnumerable
		public static IEnumerable<Vector3> IterVertices(this Obst polys) {
			return polys.Vertices();
		}

		// Returns all edges in all obstacles
		public static List<Edge> Edges(this Obst polys) {
			List<Edge> edges = new List<Edge>();
			foreach (Polygon pol in polys) {
				edges.AddRange(pol.Edges());
			}
			return edges;
		}

		// Same as Edges but returns an IEnumerable
		public static IEnumerable<Edge> IterEdges(this Obst polys) {
			return polys.Edges();
		}

		// Checks whether the given point is inside any of the obstacles
		public static bool IsInside(this Obst obstacles, Vector3 v) {
			foreach (Polygon p in obstacles) {
				if (p.IsInside(v)) {
					return true;
				}
			}
			return false;
		}

		// Checks whether the given edge intersects with any of the obstacles
		public static bool Intersects(this Obst obstacles, Edge e) {
			foreach (Polygon p in obstacles) {
				if (p.Intersects(e)) {
					return true;
				}
			}
			return false;
		}

		// Returns a list of visible vertices from this point
		public static List<Vector3> VisibleVertices(this Obst obstacles, Vector3 pos) {
			List<Vector3> vertices = new List<Vector3>();
			foreach (Vector3 v in obstacles.IterVertices()) {
				Edge e = new Edge(pos, v);
				if (!obstacles.Intersects(e)) {
					vertices.Add(v);
				}
			}
			return vertices;
		}

		// Finds all intersections of given edge and obstacles
		public static List<Vector3> Intersections(this Obst obstacles, Edge edge) {
			List<Vector3> points = new List<Vector3>();
			foreach (Polygon pol in obstacles) {
				points.AddRange(pol.Intersections(edge));
			}
			return points;
		}

		// Finds the closest edge in obstacles to given point
		// Returns both the edge and distance to that edge
		// If there are no obstacles, it returns tuple which has Edge set as null
		public static Tuple<Edge, float> ClosestWall(this Obst obstacles, Vector3 point) {
			float minDist = float.MaxValue;
			Edge minEdge = null;
			foreach (Edge e in obstacles.IterEdges()) {
				float dist = e.Distance(point);
				if (dist < minDist) {
					minDist = dist;
					minEdge = e;
				}
			}
			return Tuple.Create(minEdge, minDist);
		}

		// Finds distance to closest intersection of the edge between two given
		// points and obstacles. Returns MaxValue if it doesnt intersect.
		public static float DistToClosest(this Obst obst, Vector3 pos, Vector3 goal) {
			Edge edge = new Edge(pos, goal);
			List<Vector3> inters = obst.Intersections(edge);
			float distClosest = float.MaxValue;
			if (inters.Count > 0) {
				distClosest = inters.Min(p => Vector3.Distance(p, pos));
			}
			return distClosest;
		}

		// Finds closest intersection of an edge given by two points and the obstacles.
		// Returns the distance and the edge that hold the intersecting point.
		// If there are no intersections, returns tuple which has Edge value set to null
		public static Tuple<Edge, float> ClosestIntersection(
			this Obst obstacles, Vector3 pos, Vector3 goal) {

			Edge edge = new Edge(pos, goal);
			Edge minEdge = null;
			float minDist = float.MaxValue;
			foreach (Edge e in obstacles.IterEdges()) {
				Vector3? intPoint = edge.Intersection(e);
				if (intPoint.HasValue) {
					float dist = Vector3.Distance(intPoint.Value, pos);
					if (dist < minDist) {
						minDist = dist;
						minEdge = e;
					}
				}
			}
			return Tuple.Create(minEdge, minDist);
		}

		// Returns a polygon that contains given edge
		public static Polygon FindPolygon(this Obst obstacles, Edge e) {
			foreach (Polygon p in obstacles) {
				if (p.ContainsEdge(e)) {
					return p;
				}
			}
			return null;
		}

		// Same as FindPolygon but it will return null if given Edge is null
		// instead of throwing exception
		public static Polygon FindPolygonNull(this Obst obstacles, Edge e) {
			if (e == null) {
				return null;
			}
			return obstacles.FindPolygon(e);
		}

	}//End class
}//End namespace
