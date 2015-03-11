using UnityEngine;
using System.Collections;

public class BallTest : MonoBehaviour {

	GameObject vehicle;
	Vector3 goal;
	float slowing = 1;
	float acc = 2;
	Vector3 vel;
	Vector3 pos;

	// Use this for initialization
	void Start () {
		vehicle = Instantiate(
			Resources.Load("GameObjects/SphericalVehicle") as GameObject
		) as GameObject;
		vehicle.transform.localScale = new Vector3(1,1,1);
		pos = Vector3.zero;
		vehicle.transform.position = pos;
		vehicle.transform.parent = transform;
		vehicle.SetActive(true);
		goal = new Vector3(15, 0, 5);
		vel = new Vector3(0,0,10);
	}
	
	// Update is called once per frame
	void Update () {
		pos = vehicle.transform.position;
		float dist = Vector3.Distance(pos, goal);
		slowing = Mathf.Max(0.5f * vel.magnitude * vel.magnitude / acc, 3);
		Vector3 desired =  (goal - pos).normalized * (vel.magnitude + acc*Time.deltaTime);
		if (dist < slowing) {
			desired /= slowing;
		}
		Vector3 steer = desired - vel;
		vel += Mathf.Min(acc*Time.deltaTime, steer.magnitude) * steer.normalized;
		vehicle.transform.Translate(vel * Time.deltaTime, Space.World);
		print(dist + " " + vel.magnitude);
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(goal, 0.4f);
	}
}
