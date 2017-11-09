using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Platformer2DUserControl : MonoBehaviour
{
	public Rigidbody2D player;
	public float speed;
	public Animator	anim;
	private float gravity = 0.4f;
	private float vg = 0;
	private bool  allowJump = false;
	private int test = 0;
	private bool facingRight = true;
	private float h = 0;

    private void Awake()
    {
		player = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
    }


    private void Update()
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
		}
		if (!allowJump) {
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
