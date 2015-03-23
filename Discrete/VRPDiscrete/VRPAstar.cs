using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class VRPAStar {

	// Graph to search on
	private readonly DiscreteVRPMap map;

	// Reservation table of space time astar
	private HashSet<State> reservationTable;

	// This is heuristic function
	private Func<Vector3, Vector3, float> h;

	// Cost of the path
	public readonly float cost;

	// Paths from the start to goal for all vehicles
	public readonly List<State>[] paths;

	// Depth to search
	private int depth;


	// Constructor, also runs astar search
	// Runs AStar for each vehicle taking account reservation table for each
	// next iteration. Order of running astars is important.
	public VRPAStar(DiscreteVRPMap map, int depth, Func<Vector3, Vector3, float> h) {
		this.depth = depth;
		this.map = map;
		this.h = h;
		this.reservationTable = new HashSet<State>();
		
		int N = map.N;
		this.paths = new List<State>[N];
		Vector3[] starts = map.starts;
		List<Vector3>[] goals = map.goals;
		List<int>[] pauses = new List<int>[N];		// For cost only
		for (int i = 0; i < N; i++) {			// Run astars, set paths

			// If no customers to visit then add its start location
			if (goals[i].Count == 0) {
				goals[i].Add(starts[i]);
			}

			// First run of astar
			State start = new State(starts[i], 0);
			Node fNode = new Node(start, 0.0f, h(starts[i], goals[i][0]), null);
			Node goalNode = goals[i].Count > 1 ?
				Astar(fNode, goals[i][0], false) : Astar(fNode, goals[i][0], true);
			this.paths[i] = new List<State>(Trace(goalNode.prev));

			// Other runs of astar
			for (int j = 1; j < goals[i].Count-1; j++) {
				goalNode.prev = null;
				goalNode = Astar(goalNode, goals[i][j], false);
				Debug.Log(goalNode);
				this.paths[i].AddRange(Trace(goalNode.prev));
			}

			// Last run
			if (goals[i].Count > 1) {
				goalNode.prev = null;
				goalNode = Astar(goalNode, goals[i][goals[i].Count-1], true);
				this.paths[i].AddRange(Trace(goalNode));
			}
			
			// Check all pauses
			pauses[i] = new List<int>();
			for (int j = 0; j < depth-1; j++) {
				if (this.paths[i][j].pos.Equals(this.paths[i][j+1].pos)) {
					pauses[i].Add(j);
				}
			}
		}

		// Compute cost
		HashSet<int> inters = new HashSet<int>(pauses[0]);
		for (int i = 1; i < N; i++) {
			inters.IntersectWith(pauses[i]);
		}
		this.cost = Enumerable.Min(inters);
	}

	// Runs astar search and saves final node if exists
	private Node Astar(Node fNode, Vector3 gPos, bool last) {
		// Initializing open and closed lists
		IDictionary<State, Node> closed = new Dictionary<State, Node>();
		List<Node> open = new List<Node>();
		open.Add(fNode);

		while (open.Count > 0) {
			// Take the best node out
			Node curr = open[0];
			open.RemoveAt(0);

			if (!last && curr.state.pos.Equals(gPos)) {
				return curr;
			}

			if (curr.state.t >= depth) {		// Depth reached, finish search
				return curr;
			}

			// Move current to closed set
			closed[curr.state] = curr;
			
			// Iterate over all adjacent nodes of current node
			foreach (Tuple<State, float> sc in map.Successors(curr.state)) {
				State s = sc._1;
				float sCost = sc._2;


				// Pause command costs 0 if it is at goal
				if (curr.state.pos.Equals(gPos) && s.pos.Equals(gPos) && last) {
					sCost = 0.0f;
				}

				// This checks if there is overlap in reservation table
				// or if vehicles go through each other
				if (reservationTable.Contains(s)
					||
					(reservationTable.Contains(new State(s.pos, s.t-1))
					&& reservationTable.Contains(new State(curr.state.pos, s.t)))) {
					
					continue;
				}

				Node temp = new Node(s, curr.g + sCost, h(s.pos, gPos), curr);

				// Check if the state exists in closed list
				if (closed.ContainsKey(s)) {
					Node cNode = closed[s];
					if (temp.g < cNode.g) {
						closed.Remove(s);
					} else {
						continue;
					}
				}

				// Check if the state exists in open list
				if (open.Contains(temp)) {
					int idx = open.IndexOf(temp);
					Node oNode = open[idx];
					if (temp.g < oNode.g) {
						open.RemoveAt(idx);
					} else {
						continue;
					}
				}

				// Add the node to the open list, insert to correct position
				int ins = open.BinarySearch(temp, Node.costComp);
				if (ins < 0) {
					ins = ~ins;
				}
				open.Insert(ins, temp);
			}
		}
		return null;
	}


	// Traces path if it exists
	private List<State> Trace(Node goalNode) {
		if (goalNode == null) {			// Path does not exist
			return new List<State>();
		}

		// Trace back
		List<State> path = new List<State>();
		Node temp = goalNode;
		while (temp != null) {
			path.Add(temp.state);
			this.reservationTable.Add(temp.state);		// Fill the table
			temp = temp.prev;
		}
		path.Reverse();
		return path;
	}


	// Class for wrapping states into nodes
	private class Node {

		// Comparer used to sort nodes in the open list
		public readonly static IComparer<Node> costComp = new CostComparer();

		// State that the node holds
		public State state;

		// Total cost to get to this node
		public float g;

		// Heuristic value from this state to goal state
		// This variable is not necessary but it is used for caching
		public float h;

		// Node from which I moved to this node
		public Node prev;

		// Constructor
		public Node(State state, float g, float h, Node prev) {
			this.state = state;
			this.g = g;
			this.h = h;
			this.prev = prev;
		}

		// Overriding Equals so that nodes are equal if they hold the same state
		public override bool Equals(object other) {
			if (!(other is Node)) {
				return false;
			}
			Node o = (Node) other;
			return this.state.Equals(o.state);
		}

		// Overriding GetHashCode so that compiler will stop complaining
		public override int GetHashCode() {
			return this.state.GetHashCode();
		}

		// Overriding ToString for easier debugging
		public override String ToString() {
			return state.ToString();
		}
	}

	// No idea how to use anonymous classes, so I had to make
	// node Comparer this way
	private class CostComparer : IComparer<Node> {

		// Compares cost as both g and h
		public int Compare(Node o1, Node o2) {
			return (int) (o1.g + o1.h - o2.g - o2.h);
		}
	}
}
