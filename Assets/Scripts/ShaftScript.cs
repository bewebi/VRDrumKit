// ShaftScript.cs
// Bernie Birnbaum (c) 2016
// Gemsense Virtual Reality Drum Kit

using UnityEngine;
using System.Collections;

public class ShaftScript : MonoBehaviour {

	private float speed;
	private Vector3 velocityVector;
	private Vector3 prevPos;

	private float volume;

	// Use this for initialization
	void Start () {
		speed = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		velocityVector = transform.position - prevPos;
		speed = velocityVector.magnitude / Time.deltaTime; // Not really used atm
		prevPos = transform.position;
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Shaft")) {
			float netSpeed = ((velocityVector - other.gameObject.GetComponent<ShaftScript>().velocityVector).magnitude / Time.deltaTime);
			this.GetComponent<AudioSource>().volume = netSpeed / 20f;
			this.GetComponent<AudioSource>().Play();
		}
	}
}
