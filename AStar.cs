using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AStar {

	// Graph to search on
	private readonly Map map;

	private Func<State, float> h;

	// This is the variable to save goal node, from this cost can be
	// extracted and path traced
	private Node goalNode;

	// Cost of the path
	public float cost { get; private set; }

	// Path from the start to goal
	public List<State> path { get; private set; }


	// Constructor, also runs astar search
	public AStar(Map map) {
		this.map = map;
		this.goalNode = null;
		this.path = null;
		this.cost = -1.0f;
		Astar();
		Trace();
	}
	public IEnumerable<State> closed;
	// Runs astar search and saves final node if exists
	private void Astar() {
		// Initializing open and closed lists
		IDictionary<State, Node> closed = new Dictionary<State, Node>();
		List<Node> open = new List<Node>();
		State start = map.Initial();
		open.Add(new Node(start, 0.0f, map.h(start), null));

		int i = 0;
		while (open.Count > 0) {
			i++;
			if (i > 2000) {
				Debug.Log(closed.Count + " " + open.Count);
				this.closed = closed.Keys;
				break;
			}
			// Take the best node out
			Node curr = open[0];
			open.RemoveAt(0);

			if (map.Goal(curr.state)) {		// Found path
				goalNode = curr;
				cost = goalNode.g;
				return;
			}

			// Move current to closed set
			closed[curr.state] = curr;
			
			// Iterate over all adjacent nodes of current node
			foreach (Tuple<State, float> sc in map.Successors(curr.state)) {
				State s = sc._1;
				Node temp = new Node(s, curr.g + sc._2, map.h(s), curr);

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
	}

	// Traces path if it exists
	private void Trace() {
		if (goalNode == null) {			// Path does not exist
			return;
		}

		// Trace back
		this.path = new List<State>();
		Node temp = goalNode;
		while (temp != null) {
			path.Add(temp.state);
			temp = temp.prev;
		}
		path.Reverse();
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
		public override bool Equals(System.Object other) {
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
