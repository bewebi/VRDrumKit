// HihatScript.cs
// Bernie Birnbaum (c) 2016
// Gemsense Virtual Reality Drum Kit

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GemSDK.Unity;
using System.Runtime.InteropServices;
using System;

public class HihatScript : MonoBehaviour {

	// Hi-Hat: D0:B5:C2:90:78:E9
	public string Address;
	public Text stateText;

	private IGem gem;

	private Quaternion rotation;
	private Quaternion initialRotation;

	private float aggregateXRotation;
	private int prevHihat;
	private float upperHatDeltaHeight;
	private static float  upperHatInitialHeight;


	void Start () {
		GemManager.Instance.Connect ();
		gem = GemManager.Instance.GetGem(Address);
		upperHatInitialHeight = this.transform.Find("Top").gameObject.transform.localPosition.y;
	}

	void FixedUpdate () {
		if (gem != null)
		{
			// Will calibrate even if dropdown is active
			if (Input.GetMouseButton(0))
			{
				gem.CalibrateAzimuth();
				initialRotation = gem.Rotation;
			}

			rotation = gem.Rotation;
			rotation = (new Quaternion(rotation.x,0, 0, rotation.w)); // Only care about motion on x axis
			stateText.text = gem.State.ToString ();

			// Use latest calibration point as reference point
			aggregateXRotation = rotation.eulerAngles.x - initialRotation.eulerAngles.x;
			if(aggregateXRotation < 0) {
				aggregateXRotation += 360;
			}

			// Determine how 'open' hats are based on rotation
			prevHihat = this.transform.GetComponent<DrumNoise>().hihatLevel;	
			if(aggregateXRotation > 10 && aggregateXRotation < 340) {
				this.transform.GetComponent<DrumNoise>().hihatLevel = 4;
			} else if (aggregateXRotation > 10 && aggregateXRotation < 345) {
				this.transform.GetComponent<DrumNoise>().hihatLevel = 3;
			} else if (aggregateXRotation > 10 && aggregateXRotation < 350) {
				this.transform.GetComponent<DrumNoise>().hihatLevel = 2;
			} else if (aggregateXRotation > 10 && aggregateXRotation < 355) {
				this.transform.GetComponent<DrumNoise>().hihatLevel = 1;
			} else {
				this.transform.GetComponent<DrumNoise>().hihatLevel = 0;
			}

			// Update sounds if necessary 
			if(this.transform.GetComponent<DrumNoise>().hihatLevel != prevHihat) {
				this.transform.GetComponent<DrumNoise>().updateSounds();
			}

			if(aggregateXRotation > 339) {
				upperHatDeltaHeight = (360 - aggregateXRotation) / 100; // scales to value between 0.0 and 0.2
			} else if (aggregateXRotation > 180) {
				upperHatDeltaHeight = 0.2f;
			} else {
				upperHatDeltaHeight = 0;
			}

			// Position the upper hat based on rotation 
			Vector3 upperLocalPos = this.transform.Find("Top").transform.localPosition;
			upperLocalPos.y = upperHatInitialHeight + upperHatDeltaHeight;
			this.transform.Find("Top").transform.localPosition = upperLocalPos;
		}
	}

	void OnApplicationQuit(){
		GemManager.Instance.Disconnect();
	}

	//For Android to unbind Gem Service when the app is not in focus
	void OnApplicationPause(bool paused){
		if (Application.platform == RuntimePlatform.Android)
		{
			if (paused)
				GemManager.Instance.Disconnect();
			else
				GemManager.Instance.Connect();
		}
	}
}