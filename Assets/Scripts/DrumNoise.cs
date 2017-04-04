// DrumNoise.cs
// Bernie Birnbaum (c) 2016
// Gemsense Virtual Drum Kit

using UnityEngine;
using System.Collections;

public class DrumNoise : MonoBehaviour {

	public AudioClip[] sounds;

	// variables for drum settings
	public string drumType; // Must be set in Unity
	public int pitchLevel;  
	public int snareLevel;
	public int hihatLevel;

	// variables for settings dropdown (see TipBehavior.cs); all must be set in Unity
	public bool hasPitch; // ie not a cymbal
	public bool hasSnare; // ie is a snare drum
	public int drumCategory; // 0 - snare, 1 - hihat, 2 - tomtom, 3 - cymbal, 4 - bass, 5 - floortom
	public int drumNumber;   // index of drum (alphabetically) within drumCategory

	public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip;
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol;
		return newAudio;
	}

	public void Awake() {
		sounds = new AudioClip[6]; // Four sounds standard plus two spots for special hihat/snare sounds
		updateSounds();
	}

	// Activate the given clip from sounds
	public void setAudio(int clipNumber) {
		GetComponent<AudioSource>().clip = sounds[clipNumber];
	}

	// Reflect change in pitch setting 
	public void setPitchAndUpdate(int newPitch) {
		pitchLevel = newPitch;
		updateSounds();

		// If a pitch setting is chosen that the drum does not have, search within one for one the drum does have
		if(!(sounds[0])) {
			pitchLevel--;
			updateSounds();

			if(!(sounds[0])) {
				pitchLevel += 2;
				updateSounds();
			}
		}
	}

	// Reflect change in snare setting
	public void setSnareAndUpdate(int newSnare) {
		snareLevel = newSnare;
		updateSounds();
	}

	public void setHihat(int newHihat) {
		hihatLevel = newHihat;
	}

	public void updateSounds() {
		// filename is in format: drumType[pitchLevel][snareLevel][hihatLevel]number
		// If filename is not found, drum will simply not sound

		// drumType options:
		// tomtom8, tomtom10, tomtom12, floortom, bass, splash 8, splash12, snare14, crash 17, crash18, hihat13, hihat18, china20, ride13, crashride
		string filename = drumType;

		// Pitch key:
		// vlp: 0
		// lp:  1
		// mlp: 2
		// mp:  3
		// mhp: 4
		// hp:  5
		// vhp: 6

		// tomtom8 options:  0,1,2,3,4,5,6
		// tomtom10 options: 1,3,5,6
		// tomtom12 options: 1,3,5,6
		// bass options:     1,2,3,4,5,6
		// floortom options: 0,1,2,3,4,5,6
		// snare13 options:  1,2,3,4,5,6
		// snare14 options:  0,1,2,3,4,5,6
		if(drumType.Equals("tomtom8") || drumType.Equals("tomtom10") || drumType.Equals("tomtom12") || drumType.Equals("bass") || 
			drumType.Equals("floortom") || drumType.Equals("snare13") || drumType.Equals("snare14")) {
			filename += pitchLevel;
		}

		// Snare key:
		// 0sn: 0
		// Lsn: 1
		// Tsn: 2

		// snare13 options: 0,1,2
		// snare14 options: 0,1,2
		if(drumType.Equals("snare13") || drumType.Equals("snare14")) {
			filename += snareLevel;
		}

		// Hihat key:
		// cl: 0
		// sc: 1
		// ho: 2
		// so: 3
		// o:  4

		// hihat13 options: 0,1,2,3,4
		// hihat18 options: 0,1,2,3,4
		if(drumType.Equals("hihat13") || drumType.Equals("hihat18")) {
			filename += hihatLevel;
		}

		sounds[0] = (AudioClip)Resources.Load((filename + '0'), typeof(AudioClip));
		sounds[1] = (AudioClip)Resources.Load((filename + '1'), typeof(AudioClip));
		sounds[2] = (AudioClip)Resources.Load((filename + '2'), typeof(AudioClip));
		sounds[3] = (AudioClip)Resources.Load((filename + '3'), typeof(AudioClip));
		sounds[4] = (AudioClip)Resources.Load((filename + '4'), typeof(AudioClip)); // rimshot/footchk if applicable
		sounds[5] = (AudioClip)Resources.Load((filename + '5'), typeof(AudioClip)); // crossshot/footsplash if applicable
	}
}