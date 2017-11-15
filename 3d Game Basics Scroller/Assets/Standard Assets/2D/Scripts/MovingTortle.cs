using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTortle : MonoBehaviour {

	public GameObject	platform;
	public GameObject start;
	public GameObject end;
	public float			speed;
	private bool right = false;

	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (right) {
			Vector3 tmp = platform.transform.position;
			tmp.x += speed;
			if (tmp.x >= end.transform.position.x) {
				right = false;
				Vector3 tmp2 = platform.transform.localScale;
				tmp2.x *= -1;
				platform.transform.localScale = tmp2;
			}
			platform.transform.position = tmp;
		} else {
			Vector3 tmp = platform.transform.position;
			tmp.x -= speed;
			if (tmp.x <= start.transform.position.x) {
				right = true;
				Vector3 tmp2 = platform.transform.localScale;
				tmp2.x *= -1;
				platform.transform.localScale = tmp2;
			}
			platform.transform.position = tmp;
		}
	}
}
