using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;

public class ScoresManager : MonoBehaviour {

	private String connectionString;

	private List<Score> Scores = new List<Score>();

	public GameObject scorePrefab;
	public Transform scoreParent;

	// Use this for initialization
	void Start () {

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

	public void InsertScore(int newScore) {
		using (IDbConnection dbConnection = new SqliteConnection(connectionString)) {
			dbConnection.Open ();

			using (IDbCommand dbCmd = dbConnection.CreateCommand()) {
				string sqlQuery = string.Format("INSERT INTO Scores(Score) VALUES({0})", newScore);
				Debug.Log (">>sqlQuery = " + sqlQuery);
				sqlQuery = "INSERT INTO Scores(Score) VALUES("+newScore+")";		
				Debug.Log ("sqlQuery = " + sqlQuery);
				dbCmd.CommandText = sqlQuery;
				dbCmd.ExecuteScalar ();
				dbConnection.Close ();
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
}
