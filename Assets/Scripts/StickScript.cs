// StickScript.cs
// Bernie Birnbaum (c) 2016
// Gemsense Virtual Reality Drum Kit

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GemSDK.Unity;
using System.Runtime.InteropServices;
using System;

public class StickScript : MonoBehaviour {

	// Red marker gem: C4:BE:84:08:1A:72
	// Green marker Gem: 98:7B:F3:5A:5D:AD
	public string Address;
	public Text stateText;

	private IGem gem;

	// Use this for initialization
	void Start () {
		GemManager.Instance.Connect ();
		gem = GemManager.Instance.GetGem(Address);
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (gem != null)
		{
			// TODO: Ensure other stick does not callibrate either
			if (Input.GetMouseButton(0) && !(TipBehavior.dropdownActive))
			{
				gem.CalibrateAzimuth();
				//Use instead of CalibrateAzimuth() to calibrate also tilt and elevation
				//gem.ColibrateOrigin(); 
			}
			transform.rotation = gem.Rotation;
			stateText.text = gem.State.ToString ();
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