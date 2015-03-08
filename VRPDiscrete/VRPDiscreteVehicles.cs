using UnityEngine;
using System.Collections.Generic;

public class VRPDiscreteVehicles : AbstractDiscrete {

	// Name of the file which contains a map
	public string filename;

	// Customers list, parent and gameobject
	private GameObject customerParent;
	private List<GameObject> customers;
	private GameObject customer;


	// Use this for initialization
	protected override void LocalStart () {
		customer = Resources.Load("GameObjects/Customer") as GameObject;
		DiscreteVRPMap map = new DiscreteVRPMap("Assets/_Data/VRPMaze/" + filename);

		List<Vector3> obstaclePositions = map.GetObstaclePositions();
		GenerateObstacles(obstaclePositions);
		List<Vector3> vehiclePositions = map.GetStartPositions();
		GenerateVehicles(vehiclePositions);
		List<Vector3> customerPositions = map.GetCustomerPositions();
		GenerateCustomers(customerPositions);
	}
	
	// Update is called once per frame
	void Update () {
	
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
