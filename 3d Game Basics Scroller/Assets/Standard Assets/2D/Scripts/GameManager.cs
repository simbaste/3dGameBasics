using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager GM;

	public bool pause { get; set; }

	public KeyCode jump { get; set; }
	public KeyCode forward { get; set; }
	public KeyCode backward { get; set; }
	public KeyCode left { get; set; }
	public KeyCode right { get; set; }



	void Awake() {
		print ("GameManager Awake");
		if (GM == null) {
			DontDestroyOnLoad (gameObject);
			GM = this;
		} else if (GM != this) {
			Destroy (gameObject);
			//GM = this;
		}
		pause = true;
		jump = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("jumpKey", "Space"));
		forward = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("forwardKey", "W"));
		backward = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("backwardKey", "S"));
		left = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("leftKey", "A"));
		right = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("rightKey", "D"));
	}

	void Start () {
		
	}
	
	void Update () {
		
	}
}
