using System.Collections.Generic;

public interface Map {

	// Returns enumerable of successors and costs to state s
	IEnumerable<Tuple<State, float>> Successors(State s);

	// Returns true if state s is goal state
	bool Goal(State s);

	// Returns initial state of the map
	State Initial();

	// Heuristic value from state s to goal state
	float h(State s);
}
