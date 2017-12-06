using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;

public class Score: IComparable<Score> {

	public int score { get; set;}
	public DateTime Date { get; set;}
	public int ID { get; set;}

	public Score(int id, int score, DateTime date) {
		this.score = score;
		this.Date = date;
		this.ID = id;
	}

	public int CompareTo(Score other) {
		if (other.score < this.score) {
			return (-1);
		} else if (other.score > this.score) {
			return (1);
		} else if (other.Date < this.Date) {
			return (-1);
		} else if (other.Date > this.Date) {
			return (1);
		}
		return(0);
	}
}
