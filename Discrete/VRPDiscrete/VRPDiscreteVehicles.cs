using UnityEngine;
using System.Collections.Generic;

public class VRPDiscreteVehicles : AbstractDiscrete {

	// Name of the file which contains a map
	public string filename;

	// Customers list, parent and gameobject
	protected GameObject customerParent;
	protected List<GameObject> customers;
	protected GameObject customer;

	public int depth = 100;
	
	private List<State>[] paths;

	// Use this for initialization
	protected override void LocalStart () {

		customer = Resources.Load("GameObjects/Customer") as GameObject;
		DiscreteVRPMap map = new DiscreteVRPMap("Assets/_Data/VRPMaze/" + filename);
		
		GenerateObstacles(map.GetObstaclePositions());
		GenerateVehicles(map.GetStartPositions());
		GenerateCustomers(map.GetCustomerPositions());

		VRPAStar ast = new VRPAStar(map, depth,
			delegate(Vector3 a, Vector3 b) {
				return Mathf.Abs(a.x-b.x) + Mathf.Abs(a.z-b.z);
			}
		);
		paths = ast.paths;
		cost = ast.cost;
	}
	
	// Update is called once per frame
	void Update () {
		c++;
		if (c >= F) {
			c = 0;
			int len = paths.Length;
			for (int i = 0; i < len; i++) {
				if (step < paths[i].Count) {
					vehicles[i].transform.position = paths[i][step].pos;
					foreach (GameObject cust in customers) {
						if (cust.transform.position.Equals(vehicles[i].transform.position)) {
							cust.renderer.material.color = Color.red;
						}
					}
				}
			}
			step++;
		}
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
