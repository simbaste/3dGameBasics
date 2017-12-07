using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

	Transform optionsPanel;
	Transform scorePanel;
	Transform menuPanel;
	Event keyEvent;
	Text buttonText;
	KeyCode newKey;

	bool waitingForKey;

	void Start () {
		menuPanel = transform.Find ("MainMenu");
		optionsPanel = transform.Find ("OptionsMenu");
		scorePanel = transform.Find ("ScoreMenu");
		optionsPanel.gameObject.SetActive (false);
		scorePanel.gameObject.SetActive (false);
		waitingForKey = false;

		if (menuPanel.gameObject.activeSelf) {
			FindObjectOfType<AudioManager> ().Play ("MainMenu");
			FindObjectOfType<AudioManager> ().Stop ("Menu");
			FindObjectOfType<AudioManager> ().Stop ("GameSound");
		}

		for (int i = 0; i < 5; i++) {
			if (optionsPanel.GetChild (i).name == "ForwardKey") {
				optionsPanel.GetChild (i).GetComponentInChildren<Text> ().text = GameManager.GM.forward.ToString ();
			} else if (optionsPanel.GetChild (i).name == "BackwardKey") {
				optionsPanel.GetChild (i).GetComponentInChildren<Text> ().text = GameManager.GM.backward.ToString ();
			} else if (optionsPanel.GetChild (i).name == "LeftKey") {
				optionsPanel.GetChild (i).GetComponentInChildren<Text> ().text = GameManager.GM.left.ToString ();
			} else if (optionsPanel.GetChild (i).name == "RightKey") {
				optionsPanel.GetChild (i).GetComponentInChildren<Text> ().text = GameManager.GM.right.ToString ();
			} else if (optionsPanel.GetChild (i).name == "JumpKey") {
				optionsPanel.GetChild (i).GetComponentInChildren<Text> ().text = GameManager.GM.jump.ToString ();
			}
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && !menuPanel.gameObject.activeSelf) {
			if (optionsPanel.gameObject.activeSelf || scorePanel.gameObject.activeSelf) {
				optionsPanel.gameObject.SetActive (false);
				scorePanel.gameObject.SetActive (false);
			}
			FindObjectOfType<AudioManager> ().Play ("MainMenu");
			FindObjectOfType<AudioManager> ().Stop ("Menu");
			FindObjectOfType<AudioManager> ().Stop ("GameSound");
			menuPanel.gameObject.SetActive (true);
			GameManager.GM.pause = true;
		} else if (Input.GetKeyDown (KeyCode.Escape) && (optionsPanel.gameObject.activeSelf || scorePanel.gameObject.activeSelf)) {
			FindObjectOfType<AudioManager> ().Play ("MainMenu");
			FindObjectOfType<AudioManager> ().Stop ("Menu");
			FindObjectOfType<AudioManager> ().Stop ("GameSound");
			optionsPanel.gameObject.SetActive (false);
			scorePanel.gameObject.SetActive (false);
			menuPanel.gameObject.SetActive (true);
			GameManager.GM.pause = true;
		}
	}

	public void StartGame() {
		FindObjectOfType<AudioManager> ().Stop ("MainMenu");
		FindObjectOfType<AudioManager> ().Play ("GameSound");
		menuPanel.gameObject.SetActive (false);
		GameManager.GM.pause = false;
	}

	public void OptionsGame() {
		FindObjectOfType<AudioManager> ().Stop ("MainMenu");
		FindObjectOfType<AudioManager> ().Play ("Menu");
		optionsPanel.gameObject.SetActive (true);
		menuPanel.gameObject.SetActive (false);
	}

	public void ScoreGame() {
		FindObjectOfType<AudioManager> ().Stop ("MainMenu");
		FindObjectOfType<AudioManager> ().Play ("Menu");
		scorePanel.gameObject.SetActive (true);
		menuPanel.gameObject.SetActive (false);
	}

	public void QuitGame() {
		Application.Quit ();
	}
		
	void OnGUI() {
		keyEvent = Event.current;

		if (keyEvent.isKey && waitingForKey) {
			newKey = keyEvent.keyCode;
			waitingForKey = false;
		}
	}

	public void StartAssignment(string keyName) {
		if (!waitingForKey) {
			StartCoroutine (AssignKey(keyName));
		}
	}

	public void SendText(Text text) {
		buttonText = text;
	}

	IEnumerator WaitForKey() {
		while (!keyEvent.isKey)
			yield return null;
	}

	public IEnumerator AssignKey(string keyName) {
		waitingForKey = true;

		yield return WaitForKey();

		switch (keyName) {
		case "forward":
			GameManager.GM.forward = newKey;
			buttonText.text = GameManager.GM.forward.ToString ();
			PlayerPrefs.SetString ("forwardKey", GameManager.GM.forward.ToString ());
			break;
		case "backward":
			GameManager.GM.backward = newKey;
			buttonText.text = GameManager.GM.backward.ToString ();
			PlayerPrefs.SetString ("backwardKey", GameManager.GM.backward.ToString ());
			break;
		case "left":
			GameManager.GM.left = newKey;
			buttonText.text = GameManager.GM.left.ToString ();
			PlayerPrefs.SetString ("leftKey", GameManager.GM.left.ToString ());
			break;
		case "right":
			GameManager.GM.right = newKey;
			buttonText.text = GameManager.GM.right.ToString ();
			PlayerPrefs.SetString ("rightKey", GameManager.GM.right.ToString ());
			break;
		case "jump":
			GameManager.GM.jump = newKey;
			buttonText.text = GameManager.GM.jump.ToString ();
			PlayerPrefs.SetString ("jumpKey", GameManager.GM.jump.ToString ());
			break;
		}
		yield return null;
	}
}
