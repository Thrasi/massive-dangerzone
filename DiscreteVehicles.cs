using UnityEngine;
using System.Collections;

public class DiscreteVehicles : MonoBehaviour {

	// Object to use for vehicles
	public GameObject vehicle;

	// Name of the file which contains a map
	public string filename;


	// Use this for initialization
	void Start () {
		//DiscreteMap map = new DiscreteMap(filename);
		GameObject v = Instantiate(vehicle) as GameObject;
		v.SetActive(true);
		print(v.renderer.material.color);
		v.renderer.material.color = Color.green;
		print(v.renderer.material.color);
		v = Instantiate(vehicle) as GameObject;
		v.transform.position = new Vector3(2, 1, 2);
		print(v.renderer.material.color);
		v.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
}
