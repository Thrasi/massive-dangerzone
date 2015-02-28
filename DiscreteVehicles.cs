using UnityEngine;
using System.Collections;

public class DiscreteVehicles : MonoBehaviour {

	// Name of the file which contains a map
	public string filename;

	// Use this for initialization
	void Start () {
		DiscreteMap map = new DiscreteMap(filename);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
}
