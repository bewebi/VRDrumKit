// TipBehavior.cs
// Bernie Birnbaum (c) 2016
// Gemsense Virtual Reality Drum Kit

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class TipBehavior : MonoBehaviour {

	public float speed;
	private Vector3 velocityVector;
	private Vector3 prevPos;

	public static bool dropdownActive;
	private bool dropdownLock;
	private int hoverCount;
	private int drumCategory;

	void Start () {
		speed = 0;
		dropdownLock = false;
	}
	
	void FixedUpdate () {
		velocityVector = transform.position - prevPos;
		speed = velocityVector.magnitude / Time.deltaTime;
		prevPos = transform.position;
	}

	void OnTriggerEnter(Collider other) {
		// Handle collision with Drum object
		if(other.gameObject.CompareTag("Drum") && (velocityVector[1] < 0)) { // Collisions from above only

			// Find distance from point of contact on face of drum to center of drum
			Vector3 otherPos = other.gameObject.GetComponent<Transform>().position;
			otherPos = Quaternion.Inverse(other.gameObject.GetComponent<Transform>().rotation) * otherPos; // Undo rotation
			otherPos[1] = 0; // Cancel y component

			Vector3 myPos = transform.position;
			myPos = Quaternion.Inverse(other.gameObject.GetComponent<Transform>().rotation) * myPos; // Undo rotation
			myPos[1] = 0; // Cancel y component

			float distanceFromCenter = Vector3.Distance(myPos,otherPos);
			distanceFromCenter = distanceFromCenter / (0.5f * other.gameObject.GetComponent<Transform>().localScale[0]); // Standardize range of distances to 0.0-1.0

			other.gameObject.GetComponent<AudioSource>().volume = speed / 100f; // Unscientific method for determining volume


			// Listings are "cymbal"/"snare"/"tom"
			if(distanceFromCenter > 0.93f) {
				// Set sound to "edge"/"rim"/"rim"
				other.gameObject.GetComponent<DrumNoise>().setAudio(3);
			} else if (distanceFromCenter > 0.7f) {
				// Set sound to "edge"/"edge"/"head"
				other.gameObject.GetComponent<DrumNoise>().setAudio(2);
			} else if (distanceFromCenter > 0.3f) {
				// Set sound to "bow"/"center"/"head"
				other.gameObject.GetComponent<DrumNoise>().setAudio(1);
			} else {
				// Set sound to "bell"/"center"/"head"
				other.gameObject.GetComponent<DrumNoise>().setAudio(0);
			}
			other.gameObject.GetComponent<AudioSource>().Play();
		}
	}

	void OnTriggerStay(Collider other) {
		// Dropdown menus for drum settings
		if(other.gameObject.CompareTag("Drum")) {
			hoverCount++;
			if(!dropdownActive || dropdownLock) { // All good to create dropdown if no dropdown is active or this object has a dropdown active 
				if(hoverCount > 100) { // Create dropdown if tip has been hovering

					dropdownActive = true; // static, so other instances will know not to generate a dropdown
					dropdownLock = true;   // local, so this instance knows it is the exception

					// Activate menu for changing drum type (drums can only be switched for another drum of its type)
					drumCategory = other.gameObject.GetComponent<DrumNoise>().drumCategory;
					Dropdown d = other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Drum Dropdown " + drumCategory).gameObject.GetComponent<Dropdown>();
					d.onValueChanged.AddListener(delegate {OnDrumSelect(d, other, drumCategory);});
					d.value = other.gameObject.GetComponent<DrumNoise>().drumNumber;
					d.RefreshShownValue();
					other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Drum Dropdown " + drumCategory).gameObject.SetActive(true);

					// Activate menu for changing drum pitch
					if(other.gameObject.GetComponent<DrumNoise>().hasPitch) {
						Dropdown p = other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Pitch Dropdown").gameObject.GetComponent<Dropdown>();
						p.onValueChanged.AddListener(delegate {OnPitchSelect(p, other);});
						p.value = other.gameObject.transform.GetComponent<DrumNoise>().pitchLevel;
						p.RefreshShownValue();
						other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Pitch Dropdown").gameObject.SetActive(true);
					}

					// Activate menu for changing snare level
					if(other.gameObject.GetComponent<DrumNoise>().hasSnare) {
						Dropdown s = other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Snare Dropdown").gameObject.GetComponent<Dropdown>();
						s.onValueChanged.AddListener(delegate {OnSnareSelect(s, other);});
						s.value = other.gameObject.transform.GetComponent<DrumNoise>().snareLevel;
						s.RefreshShownValue();
						other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Snare Dropdown").gameObject.SetActive(true);
					}
				}
			}
		}
	}

	void OnDrumSelect (Dropdown d, Collider other, int drumCategory) {
		GameObject currentDrum = other.gameObject;
		GameObject newDrum = currentDrum;
		foreach(Transform child in other.transform.parent) { // iterate through all drums
			if(child.gameObject.CompareTag("Drum")) { // eliminates canvas object
				if(child.gameObject.GetComponent<DrumNoise>().drumCategory == drumCategory &&
				   child.gameObject.GetComponent<DrumNoise>().drumNumber == d.value) {
					newDrum = child.gameObject; // set newDrum to drum with specified settings
				}
			}
		}

		if(!(newDrum.active)) { // only switch to a drum that is not already in use
			newDrum.transform.position = currentDrum.transform.position;
			newDrum.transform.rotation = currentDrum.transform.rotation;
			newDrum.SetActive(true);
			currentDrum.SetActive(false);
		}
	}

	void OnPitchSelect(Dropdown d, Collider other) {
		other.gameObject.GetComponent<DrumNoise>().setPitchAndUpdate(d.value);
	}

	void OnSnareSelect(Dropdown d, Collider other) {
		other.gameObject.GetComponent<DrumNoise>().setSnareAndUpdate(d.value);
	}

	// Known bug: If tip 'exits' drum as a result of a change to a smaller drum type (with the tip
	// no longer in the collider) this will not be called and the dropdown will remain visible (but
	// not functional) until a tip exits a drum, invoking this method
	void OnTriggerExit(Collider other) {
		// Remove dropdown menus
		if(other.gameObject.CompareTag("Drum")) {
			hoverCount = 0;

			other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Drum Dropdown " + drumCategory).gameObject.GetComponent<Dropdown>().onValueChanged.RemoveAllListeners();
			other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Drum Dropdown " + drumCategory).gameObject.SetActive(false);

			if(other.gameObject.GetComponent<DrumNoise>().hasPitch) {
				other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Pitch Dropdown").gameObject.GetComponent<Dropdown>().onValueChanged.RemoveAllListeners();
				other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Pitch Dropdown").gameObject.SetActive(false);
			}

			if(other.gameObject.GetComponent<DrumNoise>().hasSnare) {
				other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Snare Dropdown").gameObject.GetComponent<Dropdown>().onValueChanged.RemoveAllListeners();
				other.gameObject.transform.root.FindChild("Canvas").gameObject.transform.FindChild("Snare Dropdown").gameObject.SetActive(false);
			}

			dropdownActive = false;
			dropdownLock = false;
		}
	}
}