﻿using UnityEngine;
using System.Collections.Generic;

public class VRPDiscreteVehicles : AbstractDiscrete {

	// Name of the file which contains a map
	public string filename;

	// Customers list, parent and gameobject
	protected GameObject customerParent;
	protected List<GameObject> customers;
	protected GameObject customer;
	

	// Use this for initialization
	protected override void LocalStart () {

		customer = Resources.Load("GameObjects/Customer") as GameObject;
		DiscreteVRPMap map = new DiscreteVRPMap("Assets/_Data/VRPMaze/" + filename);


		GenerateObstacles(map.GetObstaclePositions());
		GenerateVehicles(map.GetStartPositions());
		GenerateCustomers(map.GetCustomerPositions());
//		int V = 2, N = 4;
//		System.Random rand = new System.Random ();

//		float [,] dists = new float[N+V,N+V];
//		for (int i=0; i<N+V; i++) {
//			for (int j=0;j<N+V;j++){
//				dists[i,j] = 1;//VRPSolver.NextFloat(rand);
//			}		
//		}
//		float [,] dists = new float[,] {{0f,1f,4f,5f,2f,3f},
//										{1f,0f,3f,4f,1f,2f},
//										{4f,3f,0f,1f,2f,1f},
//										{5f,4f,1f,0f,3f,2f},
//										{2f,1f,2f,3f,0f,1f},
//										{3f,2f,1f,2f,1f,0f}};



//		VRP vrp = new VRP (N, V, dists);
//
		VRPSolver vrps = new VRPSolver (map.vrp);

		DiscreteMap discreteMap = new DiscreteMap("Assets/_Data/ColAvoidMaze/" + filename);

//		Debug.Log("hallo");
	}
	
	// Update is called once per frame
	void Update () {

	}

	// Generate customers
	protected void GenerateCustomers(List<Vector3> positions) {
		customerParent = new GameObject();
		customerParent.name = "Customers";
		customers = new List<GameObject>();
		foreach (Vector3 pos in positions) {
			GameObject obj = Instantiate(customer) as GameObject;
			obj.transform.position = pos;
			obj.transform.parent = customerParent.transform;
			obj.SetActive(true);
			customers.Add(obj);
		}
	}

}
