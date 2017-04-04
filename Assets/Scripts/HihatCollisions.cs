// HihatCollisions.cs
// Bernie Birnbaum (c) 2016
// Gemsense Virtual Reality Drum Kit

using UnityEngine;
using System.Collections;

public class HihatCollisions : MonoBehaviour {

	private int durationCount;
	private bool inContact;
	private Vector3 velocityVector;
	private Vector3 prevPos;
	private float speed;
	public float capturedSpeed;
	
	// Update is called once per frame
	void FixedUpdate () {
		velocityVector = transform.position - prevPos;
		speed = velocityVector.magnitude / Time.deltaTime;
		prevPos = transform.position;
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Hihat")) {
			inContact = true;
			capturedSpeed = speed;
		}
	}

	void OnTriggerStay(Collider other) {
		if(other.gameObject.CompareTag("Hihat") && inContact) {
			durationCount++;
			// On the third fram start playing footchk
			if(durationCount == 3) {
				this.transform.parent.gameObject.GetComponent<DrumNoise>().setAudio(4);
				if(capturedSpeed > 0.005f) {
					this.transform.parent.gameObject.GetComponent<AudioSource>().volume = capturedSpeed * 2.5f;
				}
				this.transform.parent.gameObject.GetComponent<AudioSource>().Play();
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag("Hihat") && inContact) {
			// If hihats are only in contact briefly, play footsplash
			if(durationCount < 5) {
				this.transform.parent.gameObject.GetComponent<DrumNoise>().setAudio(5);
				if(capturedSpeed > 0.005f) {
					this.transform.parent.gameObject.GetComponent<AudioSource>().volume = capturedSpeed * 3f;
				}
				this.transform.parent.gameObject.GetComponent<AudioSource>().Play();
			}
			inContact = false;
			durationCount = 0;
		}
	}
}