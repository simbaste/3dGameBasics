using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

	public GameObject score;
	public GameObject rank;
	public GameObject date;

	public void SetScore(string rank, string score, string date) {
		this.rank.GetComponent<Text> ().text = rank;
		this.score.GetComponent<Text> ().text = score;
		this.date.GetComponent<Text> ().text = date;
	}
}
