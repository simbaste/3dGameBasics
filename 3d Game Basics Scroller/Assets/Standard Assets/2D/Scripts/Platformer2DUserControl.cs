using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;

public class Platformer2DUserControl : MonoBehaviour
{
	public Rigidbody2D player;
	public float speed;
	public Animator	anim;
	private float gravity = 0.4f;
	private float vg = 0;
	private bool  allowJump = false;
	private bool facingRight = true;
	private float h = 0;
	private List<GameObject> patterns = new List<GameObject>();
	public GameObject pattern1;

    private void Awake()
    {
		player = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		GameObject tmp = Instantiate (pattern1, transform.position, transform.rotation);
		patterns.Add (tmp);
		Vector3 vec = transform.position;
		vec.x += 10;
		tmp = Instantiate (pattern1, vec, transform.rotation);
		patterns.Add (tmp);
    }


    private void Update()
    {
		Vector3 playerPos = player.transform.position;
		playerPos.x -= 0.01f;
		player.transform.position = playerPos;
		foreach (GameObject obj in patterns)
		{
			Vector3 test = obj.transform.position;
			test.x -= 0.01f;
			obj.transform.position = test;
		}

		//patterns.Add (Instantiate (tmp));
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
