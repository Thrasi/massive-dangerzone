﻿using UnityEngine;
using System.Collections.Generic;

public class VRPPolygonalVehicles : AbstractVehicles {

	// Customers list, parent and gameobject
	private GameObject customerParent;
	private List<GameObject> customers;
	private GameObject customer;

	// Name of the file for the map
	public string filename;

	// Material for obstacles
	private Material material;


	// Use this for initialization
	void Start () {
		material = Resources.Load("Materials/ObstacleMaterial") as Material;
		vehicle = Resources.Load("GameObjects/LargeVehicle") as GameObject;
		customer = Resources.Load("GameObjects/LargeCustomer") as GameObject;
		VRPPolygMap map = new VRPPolygMap("Assets/_Data/VRPPolyg/" + filename);
		
		GenerateObstacles(map.polys);
		GenerateVehicles(new List<Vector3>(map.starts));
		GenerateCustomers(map.GetCustomerPositions());
	}
	
	// Update is called once per frame
	void Update () {
	}

	// Generate and render all obstacles
	private void GenerateObstacles(IEnumerable<Polygon> polys) {
		GameObject parent = new GameObject();
		parent.name = "Polygonal Obstacles";
		foreach (Polygon pol in polys) {
			GameObject go = pol.ToGameObject(material);
			go.transform.parent = parent.transform;
		}
	}

	// Generate customers
	private void GenerateCustomers(List<Vector3> positions) {
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