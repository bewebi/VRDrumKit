// PedalScript.cs
// Bernie Birnbaum (c) 2016
// Gemsense Virtual Reality Drum Kit

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GemSDK.Unity;
using System.Runtime.InteropServices;
using System;

public class PedalScript : MonoBehaviour {

	// Bass pedal gem: D0:B5:C2:90:7C:53
	public string Address;
	public Text stateText;

	private Quaternion initialRotation;
	private IGem gem;

	void Start () {
		GemManager.Instance.Connect ();
		gem = GemManager.Instance.GetGem(Address);
	}

	void FixedUpdate () {
		if (gem != null)
		{
			if (Input.GetMouseButton(0))
			{
				gem.CalibrateAzimuth();
				initialRotation = gem.Rotation;
			}

			// Use most recent calibrate point as reference point
			transform.rotation = gem.Rotation * (Quaternion.Inverse(initialRotation));
			transform.rotation = (new Quaternion(transform.rotation.x,0, 0, transform.rotation.w)); // Only care about motion on x axis
			stateText.text = gem.State.ToString();
		}
	}

	void OnApplicationQuit(){
		GemManager.Instance.Disconnect();
	}

	// For Android to unbind Gem Service when the app is not in focus
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