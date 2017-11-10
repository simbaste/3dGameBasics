using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;

public class Platformer2DUserControl : MonoBehaviour
{
	public Rigidbody2D player;
	public float speed;
	public Animator	anim;
	private float scrollSpeed = 0.015f;
	private float gravity = 0.4f;
	private float vg = 0;
	private bool  allowJump = false;
	private bool facingRight = true;
	private float h = 0;
	private List<GameObject> patterns = new List<GameObject>();
	private List<GameObject> level = new List<GameObject>();
	public GameObject start;
	public GameObject pattern1;
	public GameObject pattern2;
	public GameObject pattern3;
	public GameObject pattern4;
	private System.Random rnd = new System.Random();
	private int last = 0;
	private int score = 1;

    private void Awake()
    {
		player = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		GameObject tmp;
		patterns.Add (pattern1);
		patterns.Add (pattern2);
		patterns.Add (pattern3);
		patterns.Add (pattern4);

		level.Add (Instantiate (start, start.transform.position, start.transform.rotation));
		int i = 10;
		int rand = rnd.Next (4);
		Vector3 vec;
		while (level.Count < 3) {
			vec = patterns[rand].transform.position;
			vec.x += i;
			tmp = Instantiate (patterns[rand], vec, patterns[rand].transform.rotation);
			level.Add (tmp);
			i += 15;
			last = rand;
			rand = rnd.Next (4);
			if (last == rand) {
				rand = (last + 1) % patterns.Count;
			}
		}
    }


    private void Update()
    {
		++score;
		if (score % 1000 == 0) {
			scrollSpeed += 0.002f;
		}
		print (score);
		generate ();
		scrollScreen ();
		moveAndAnimation ();
    }

	private void moveAndAnimation()
	{
		h = CrossPlatformInputManager.GetAxis("Horizontal");
		Flip ();
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) {
			anim.SetInteger ("State", 1);
		} else {
			anim.SetInteger ("State", 0);
		}
		float jump = CrossPlatformInputManager.GetAxis ("Jump");
		if (jump != 0 && allowJump) {
			vg = 10;
			anim.SetInteger ("State", 2);
		}
		vg -= gravity;
		player.velocity = new Vector2 (h * speed, vg);
		allowJump = false;
	}

	private void scrollScreen()
	{
		Vector3 playerPos = player.transform.position;
		playerPos.x -= scrollSpeed;
		player.transform.position = playerPos;
		foreach (GameObject obj in level)
		{
			Vector3 test = obj.transform.position;
			test.x -= scrollSpeed;
			obj.transform.position = test;
		}

	}

	private void generate()
	{
		if (level [0].transform.position.x <= -15f) {
			Destroy (level [0]);
			level.RemoveAt (0);
			int tmpRnd = rnd.Next (4);
			if (tmpRnd == last) {
				tmpRnd = (last + 1) % patterns.Count;
			}
			last = tmpRnd;
			Vector3 vec = level [level.Count - 1].transform.position;
			vec.x += 15;
			level.Add (Instantiate(patterns [tmpRnd], vec, level [level.Count - 1].transform.rotation));
		}
	}

	private void Flip()
	{
		if ((h > 0f && !facingRight) || (h < 0f && facingRight)) {
			facingRight = !facingRight;
			Vector3 tmp = transform.localScale;
			tmp.x *= -1;
			transform.localScale = tmp;
		}
	}

	void OnCollisionStay2D(Collision2D coll)
	{
		if (coll.gameObject.CompareTag("Ground")) {
			vg = 0;
			allowJump = true;
		} else if (coll.gameObject.CompareTag("Wall")) {
			allowJump = true;
		}
	}
}
