﻿using UnityEngine;
using System.Collections;

public class AudioButton : MonoBehaviour {

	public Texture2D onIcon;
	public Texture2D offIcon;

	private static bool isAudioOn = true;

	void OnGUI () {
		if(isAudioOn){
			if (GUI.Button (new Rect (10,10,45,45), onIcon)){
				GameState.Instance.pauseMusic();
				isAudioOn = false;
			}
		}
		else{
			if (GUI.Button (new Rect (10,10,45,45), offIcon)){
				GameState.Instance.playMusic();
				isAudioOn = true;
			}
		}

	}
}
