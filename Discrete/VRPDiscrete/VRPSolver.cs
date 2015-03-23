using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class VRPSolver {
	System.Random rand = new System.Random ();
	VRP vrp;


	List< List< List<int> > > population;
	List<float> costs = new List<float>();
	float [] cumulativeCost;
	float [] cumulativeInverseCost;
	// Parameters:
	int populationSize = 100;
	public int pSame = 50;

	public VRPSolver(VRP vrp) {
		this.vrp = vrp;
		populate (populationSize);
		foreach (List<List<int>> s in population) {
			printChromosome(s);
		}
		Debug.Log ("-------------------------");
		for (int i=0; i<2000; i++) {
			steadyState ();
		}
		foreach (List<List<int>> s in population) {
			printChromosome(s);
		}


	}

	public List< List<int>> getBest() {
		float best = float.MaxValue;
		List<List<int>> bestRoute = null;
		foreach (List<List<int>> s in population) {
			float c = cost(s);
			if (c < best) {
				best = c;
				bestRoute = s;
			}
		}
		return bestRoute;
	}

	/*
	 * creates a population of size M
	 * All veihicles are assigned a randomized but equal amount of destinations
	 */
	void populate(int M) {
		population = new List< List< List<int> > > ();
		List<int> nodes = new List<int>();
		for (int j=0; j<vrp.N;j++) {
			nodes.Add(j);
		}

		int [] arr = new int[vrp.V+1];
		int c = vrp.N / vrp.V;
		Debug.Log("c: "+c+ ", V: "+vrp.V+", nodesCount: "+nodes.Count);
		arr [0] = 0;
		arr [vrp.V] = nodes.Count;
		string ss= "" + 0 + ", ";
		for (int j=1;j<arr.Length-1;j++) {
			arr[j] = j*c;
//			//Debug.Log(j*c);
			ss += j*c+", ";
		}
//		//Debug.Log(ss+ arr[arr.Length-1]);

		for (int i=0; i<M; i++) {
			List< List<int>> pop = new List< List<int>>();
			for (int j=0;j<vrp.V;j++) {
				pop.Add(new List<int>());
			}

			nodes = new List<int>();
			for (int j=0; j<vrp.N;j++) {
				nodes.Add(j);
			}
			Shuffle (nodes);

			int index = 0;
			foreach (int n in nodes) {
				pop[index++].Add(n);
				index = index % vrp.V;
			}
			population.Add(pop);
			costs.Add (cost (pop));
		}

//		for (int i=0; i<M; i++) {
//			Shuffle (nodes);
//			List< List<int>> pop = new List< List<int>>();
//
//			string s= "";
//			for (int j=1; j<arr.Length;j++) {
//				pop.Add( nodes.GetRange(arr[j-1],arr[j]-arr[j-1]) );
//				foreach (int k in pop[j-1] ) {
//					s += k+", ";
//				}
//				s += " // ";
//			}
//			Debug.Log(s);
//			population.Add(pop);
////			printChromosome(pop);
////			//Debug.Log("cost: "+cost (pop));
//			costs.Add (cost (pop));
//		}

		calcCumulative (M);
	}

	void calcCumulative(int M) {
		cumulativeCost = new float[M];
		cumulativeInverseCost = new float[M];
//		string s = "", ss = "";
		cumulativeCost [0] = costs [0];
//		s += cumulativeCost [0] + ", ";
		cumulativeInverseCost [0] = 1 / costs [0];
//		ss += cumulativeInverseCost [0] + ", ";
		for (int i=1;i<costs.Count;i++) {
			cumulativeCost [i] = cumulativeCost [i-1] + costs [i]; 
//			s+=cumulativeCost [i]+", ";
			cumulativeInverseCost [i] = cumulativeInverseCost [i-1] + 1 / costs [i]; 
//			ss+=cumulativeInverseCost[i]+", ";
		}
//		//Debug.Log ("cumulative: " + s);
//		//Debug.Log ("inverse: " + ss);
	}

	void steadyState() {
		List<List<int>> P1, P2, O1, O2, R;

		P1 = ParentSelection ();
		P2 = ParentSelection ();
//		printChromosome (P1);
//		printChromosome (P2);
	
		O1 = crossover (P1,P2);
//		printChromosome (O1);
		O2 = mutation (O1, pSame);

		R = SolutionOutSelection ();
		//Debug.Log("outSelection: ");
//		printChromosome (R);
		int index = population.IndexOf (R);
		//Debug.Log ("index of it: " + index);
		replace (R, O2);

		costs [index] = cost (R);
		calcCumulative (populationSize);
	}


	List<List<int>> crossover(List<List<int>> P1, List<List<int>> P2) {
		return SRcrossover (P1, P2);
	}
	/* A simple random cross over 
	 */
	List<List<int>> SRcrossover(List<List<int>> P1, List<List<int>> P2) {
		List<List<int>> O = new List<List<int>>();

		foreach (List<int> r in P1) {
			O.Add(new List<int>(r));  // pointer or hard copy?
		}

		int routeIndex = rand.Next (0, P2.Count);
		while (P2 [routeIndex].Count==0) {
			routeIndex = rand.Next (0, P2.Count);
		}
		List<int> route = P2 [routeIndex];

		int i = rand.Next (0, route.Count);
		int j = rand.Next (1, route.Count-i+1);
//		printRoute (route);
//		Debug.Log ("rout.count = " + route.Count + ", from " + i + " to " + (i + j));
		List<int> subRoute = route.GetRange(i,j);
		//Debug.Log ("subRoute from "+i+" to "+(i+j));
//		printRoute (subRoute);

		for (int k=0;k<O.Count;k++) {
			O[k] = O[k].Except(subRoute).ToList();
		}
		foreach (List<int> r in O) {
			 // does this work?
		}

		//Debug.Log("enter BI");
		int [] bestIndex = BestInsertion (O, subRoute);
		//Debug.Log("exit BI");
		O[bestIndex[0]].InsertRange (bestIndex[1], subRoute);

		//TODO: Here we should probably add a LSA!

		return O;
	}
	/*
	 * This payoff calculation is stupid, find a new one.
	 */

	int [] BestInsertion (List<List<int>> O, List<int> subRoute) {
		/* This doesn't allow for adding to the end of a subroute*/
		int k1 = subRoute [0];
		int k2 = subRoute [subRoute.Count - 1];
		int bestRouteIndex = 0;
		int bestCustomerIndex = 0;
		float payoff;
		float bestpayoff = float.MinValue;
		//Debug.Log ("length of O: " + O.Count);
		for (int vehicle=0; vehicle<O.Count; vehicle++) {
			int customerIndex = 0;

			List<int> route = O[vehicle];
			//Debug.Log("length of route: "+route.Count);
			if (route.Count==0 && O.Count==1) {
				continue;
			}
			if (route.Count==0 && O.Count>1) {
				payoff = - vrp.distances[vrp.N+vehicle, k1];
				if (payoff > bestpayoff) {
					bestpayoff = payoff;
					bestRouteIndex = vehicle;
					bestCustomerIndex = 0;
				}
				continue;
			}
			int customer1 = route[customerIndex];
			int customer2;

			
			payoff = vrp.distances[vrp.N+vehicle, customer1] - vrp.distances[vrp.N+vehicle, k1] - vrp.distances[k2, customer1];
			if (payoff > bestpayoff) {
				bestpayoff = payoff;
				bestRouteIndex = vehicle;
				bestCustomerIndex = 0;
			}
			
			for (customerIndex=1; customerIndex<route.Count; customerIndex++) {
				customer1 = route[customerIndex-1];
				customer2 = route[customerIndex];
				payoff = vrp.distances[customer1, customer2] - vrp.distances[customer1, k1] - vrp.distances[k2, customer2];
				
				if (payoff > bestpayoff) {
					bestpayoff = payoff;
					bestRouteIndex = vehicle;
					bestCustomerIndex = customerIndex;
				}
			}
		}
		int [] ret = {bestRouteIndex, bestCustomerIndex};
		return ret;
	}

	List<List<int>> mutation(List<List<int>> offspring, int pSame) {

		int NR = offspring.Count;
		int jMut = rand.Next (0, NR);

		while ( offspring [jMut].Count == 0) {
			jMut = rand.Next (0, NR);
		}
		int RL = offspring [jMut].Count;
		int kMut = rand.Next (0, RL);
//		Debug.Log ("jMut: " + jMut + ", kMut: " + kMut);
		int customer = offspring [jMut] [kMut];
//		Debug.Log ("jMut: " + jMut + ", kMut: " + kMut);
		//Debug.Log("Offspring to mutate:");
//		printChromosome (offspring);

		offspring [jMut].Remove (customer);
//		printChromosome (offspring);

		int prob = rand.Next (0, 101);
		int[] bestIndex;
		int j;
		List<List<int>> route = new List<List<int>> ();
		List<int> customerList = new List<int> ();
		customerList.Add (customer);
		//Debug.Log("The route taken with prob: "+prob);
		if (prob < pSame) {
				route.Add (offspring [jMut]);
//				printRoute (route[0]);
				bestIndex = BestInsertion (route, customerList);
				bestIndex [0] = jMut;
		} else {
			do {
				route = new List<List<int>> ();
				j = rand.Next (0, NR);
				route.Add (offspring [j]);
//				printRoute (route[0]);
				bestIndex = BestInsertion (route, customerList);
				bestIndex [0] = j;
			} while (j == jMut);
		}

		offspring[bestIndex[0]].Insert (bestIndex[1], customer);
//		printChromosome (offspring);
		return offspring;
	}

	List<List<int>> SolutionOutSelection() {
		/*
		 * Roulette wheel selection. Wheighted random selection.
		 */
		float value = UnityEngine.Random.Range(0, cumulativeCost[cumulativeCost.Length-1]);
		int index = Array.BinarySearch(cumulativeCost, value);
		if (index < 0) {
			index = ~index;
		}
		return population[index];
	}

	List<List<int>> ParentSelection() {
		/*
		 * Roulette wheel selection. Wheighted random selection.
		 */
		float value = UnityEngine.Random.Range(0, cumulativeInverseCost[cumulativeCost.Length-1]);
//		string s = "";
//		foreach (float fl in cumulativeInverseCost) {
//			s+=fl+", ";
//		}
//		//Debug.Log ("inverse: " + s);
//		//Debug.Log ("value: " + value);
		int index = Array.BinarySearch(cumulativeInverseCost, value);
//		//Debug.Log ("index: " + index);
		if (index < 0) {
			index = ~index;
		}
//		//Debug.Log ("index: " + index);
		return population[index];
	}

	float cost(List<List<int>> solution) {
		/* cost as the sum of the distances within the solution.
		 */

		float maxCost = 0;
		for (int vehicle=0; vehicle<solution.Count; vehicle++) {
			//Debug.Log("vehicle: "+vehicle);

			if (solution[vehicle].Count==0) {
				continue;
			}

			float f = 0;
			int from = vehicle + vrp.N;
//			Debug.Log("from: "+from);
			int to = solution[vehicle][0];
			f += vrp.distances[from,to];

			for (int i=1;i<solution[vehicle].Count;i++) {
				from = solution[vehicle][i-1];
				to = solution[vehicle][i];

				f += vrp.distances[from, to];
			}
			if ( f > maxCost ) {
				maxCost = f;
			}
//			Debug.Log("vehicle: "+vehicle+", cost: "+f);
		}
		return maxCost;
	}

	void replace(List<List<int>> R, List<List<int>> O) {
		for (int i=0;i<O.Count;i++) {
			R[i] = O[i];
		}
	}


	// stole this from the web
	void Shuffle(List<int> list)
	{
		int n = list.Count;
		int t;
		for (int i = 0; i < n; i++)
		{
			// NextDouble returns a random number between 0 and 1.
			// ... It is equivalent to Math.random() in Java.
			int r = i + (int)(rand.NextDouble() * (n - i));
			t = list[r];
			list[r] = list[i];
			list[i] = t;
		}
	}

	public void printRoute(List<int> R) {
		string output = "";
		foreach(int c in R) {
			output += c+", ";
		}
		output+="\n";
		Debug.Log (output);
	}

	public void printChromosome(List<List<int>> C) {
		string output = "";
		foreach (List<int> vehicle in C) {
			foreach(int c in vehicle) {
				output += c+", ";
			}
			output+="\n";
		}
		Debug.Log (output+"Cost: "+cost(C));
	}
}

