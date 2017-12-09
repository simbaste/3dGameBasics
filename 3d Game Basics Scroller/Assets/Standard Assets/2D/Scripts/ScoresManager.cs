using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;

public class ScoresManager : MonoBehaviour {

	private String connectionString;

	private List<Score> Scores = new List<Score>();
	Transform scorePanel;

	public GameObject scorePrefab;
	public Transform scoreParent;
	public int saveScores;

	// Use this for initialization
	void Start () {

		scorePanel = transform.Find ("ScoreMenu");
		connectionString = "URI=file:"+Application.dataPath+"/DB/3DGameScoreDB";
		if (!File.Exists (Application.dataPath+"/DB/3DGameScoreDB")) {
			using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
				dbConnection.Open ();

				using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
					string sqlQuery = "CREATE TABLE \"Scores\" (\"ScoreID\" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, \"Score\" INTEGER, \"Date\" DEFAULT CURRENT_DATE)";
					dbCmd.CommandText = sqlQuery;
					dbCmd.ExecuteScalar ();
					dbConnection.Close ();
				}

			}
		}
		Debug.Log (connectionString);
		ShowScores ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void GetScore() {
		Scores.Clear ();
		using (IDbConnection dbConnection = new SqliteConnection(connectionString)) {
			dbConnection.Open ();

			using (IDbCommand dbCmd = dbConnection.CreateCommand()) {
				string sqlQuery = "SELECT * FROM Scores";
				dbCmd.CommandText = sqlQuery;

				using (IDataReader reader = dbCmd.ExecuteReader ()) {
					while (reader.Read()) {
						//Scores.Add (new Score((int)reader.GetInt32(1), (int)reader.GetInt32(2), (DateTime)reader.GetDateTime(3)));
						Scores.Add (new Score((int)reader.GetInt32(0), (int)reader.GetInt32(1), (DateTime)reader.GetDateTime(2)));
					}
					dbConnection.Close ();
					reader.Close ();
				}
			}
		}
		Scores.Sort ();
	}

	public void LimitScoreSize() {
		scorePanel.GetChild (4).gameObject.SetActive (true);
	}

	public void SetLimitScore() {
		int scoreLimit;

		if (int.TryParse(scorePanel.GetChild (4).GetComponentInChildren<InputField>().text, out scoreLimit)) {
			if (scoreLimit > 0) {
				saveScores = scoreLimit;
				DeleteExtraScore ();
			}
		}

		foreach (RectTransform child in scorePanel.GetChild(2).GetChild(0)) {
			Destroy ((child as Transform).gameObject);
		}

		scorePanel.GetChild (4).gameObject.SetActive (false);
		ShowScores ();
	}

	public void InsertScore(int newScore) {

		GetScore ();
		int hsCount = Scores.Count;

		if (Scores.Count > 0) {
			Score lowestScore = Scores[Scores.Count - 1];

			if (lowestScore != null && saveScores > 0 && Scores.Count >= saveScores && newScore > lowestScore.score) {
				deleteScore (lowestScore.ID);
				hsCount--;
			}
		}

		if (hsCount < saveScores) {
			using (IDbConnection dbConnection = new SqliteConnection(connectionString)) {
				dbConnection.Open ();

				using (IDbCommand dbCmd = dbConnection.CreateCommand()) {
					string sqlQuery = string.Format("INSERT INTO Scores(Score) VALUES({0})", newScore);
					Debug.Log ("sqlQuery = " + sqlQuery);
					sqlQuery = "INSERT INTO Scores(Score) VALUES("+newScore+")";		
					Debug.Log ("sqlQuery = " + sqlQuery);
					dbCmd.CommandText = sqlQuery;
					dbCmd.ExecuteScalar ();
					dbConnection.Close ();
				}
			}
		}
	}

	private void ShowScores() {
		GetScore ();
		for (int i = 0; i < Scores.Count; i++) {
			GameObject tmpObject = Instantiate (scorePrefab);
			Score tmpScore = Scores[i];
			tmpObject.GetComponent<ScoreScript> ().SetScore ("#"+(i+1).ToString(), tmpScore.score.ToString(), tmpScore.Date.ToString());
			tmpObject.transform.SetParent (scoreParent);
			tmpObject.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		}
	}

	private void deleteScore(int id) {
		GetScore ();

		using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
			dbConnection.Open ();

			using(IDbCommand dbCommand = dbConnection.CreateCommand()) {

				string sqlQuery = String.Format ("DELETE FROM Scores WHERE ScoreID = \"{0}\"", id);

				dbCommand.CommandText = sqlQuery;
				dbCommand.ExecuteScalar ();
				dbConnection.Close ();
			}
		}
	}

	private void DeleteExtraScore() {
		GetScore ();
		if (saveScores <= Scores.Count) {
			int deleteCount = Scores.Count - saveScores;
			Scores.Reverse ();

			using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
				dbConnection.Open ();

				using(IDbCommand dbCommand = dbConnection.CreateCommand()) {

					for (int i = 0; i < deleteCount; i++) {
						string sqlQuery = String.Format ("DELETE FROM Scores WHERE ScoreID = \"{0}\"", Scores[i].ID);

						dbCommand.CommandText = sqlQuery;
						dbCommand.ExecuteScalar ();
					}
					dbConnection.Close ();
				}
			}
		}
	}
}
