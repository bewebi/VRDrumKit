// PedalTipBehavior.cs
// Bernie Birnbaum (c) 2016
// Gemsense Virtual Drum Kit

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class PedalTipBehavior : MonoBehaviour {

	private float velocity;
	private Vector3 velocityVector;
	private Vector3 prevPos;

	private float[] recentVelocities;
	private float maxRecentVelocity;
	private int recentCount;
	private int recentTimer;
	private int timeBetween;

	// Use this for initialization
	void Start () {
		velocity = 0;
		recentTimer = 7;
		recentVelocities = new float[recentTimer];
	}
	
	void FixedUpdate () {
		velocityVector = transform.position - prevPos;
		velocity = velocityVector.magnitude / Time.deltaTime;

		prevPos = transform.position;
		timeBetween++;

		// Moment of contact may not reflect intended velocity of strike
		// Instead remember largest velocity of recent moments
		recentVelocities[recentCount] = velocity;
		recentCount++;
		recentCount = recentCount % recentTimer;
		maxRecentVelocity = 0;
		for(int i = 0; i < recentTimer; i++) {
			if (recentVelocities[i] > maxRecentVelocity) {
				maxRecentVelocity = recentVelocities[i];
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Drum")) {
			// No need for lots of calculations, the Bass Drum is the same every time
			other.gameObject.GetComponent<AudioSource>().volume = maxRecentVelocity / 8f;
			other.gameObject.GetComponent<DrumNoise>().setAudio(0);
			
			// Collisions can inadverntanly be registered much quicker than any user would intend
			if(timeBetween > 10) { 
				other.gameObject.GetComponent<AudioSource>().Play();
				Debug.Log("timeBetween: " + timeBetween);
				timeBetween = 0;
			}
		}
	}
// Note: To change settings on Bass Drum, use standard drumstick
}
