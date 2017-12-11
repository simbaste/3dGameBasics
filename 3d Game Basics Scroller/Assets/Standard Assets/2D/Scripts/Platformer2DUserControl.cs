using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;
using UnityEngine.UI;

public class Platformer2DUserControl : MonoBehaviour
{
	public Rigidbody2D player;
	public float speed;
	public Text scoreTxt;
	public Animator	anim;
	private float scrollSpeed = 0.025f;
	private float gravity = 0.4f;
	private float vg = 0;
	private bool  allowJump = false;
	private bool facingRight = true;
	private List<GameObject> level = new List<GameObject>();
	public GameObject start;
	public GameObject[] patterns;
	public GameObject spikes;
	public GameObject explode;
	public GameObject explode_scie;
	public GameObject gameover;
	public Text scoreGameOver;
	private List<GameObject> explodes = new List<GameObject>();
	private System.Random rnd = new System.Random();
	private int last = 0;
	private int score = 0;
	private bool isStart = false;
	private bool spike = false;
	private bool die = false;
	private float bonus = 0;
	private GameObject starIcon;
	private float vh = 0;
	private int tempoDie = 0;
	private Vector3 spikeX;
	private Vector3 playerX;

    private void Awake()
    {
		Application.targetFrameRate = 60;
		spikeX = spikes.transform.position;
		playerX = player.transform.position;
		GameObject tmp;
		starIcon = player.transform.Find ("star").gameObject;

		Vector3 vec = starIcon.transform.localScale;
		vec.x = 0;
		vec.y = 0;
		starIcon.transform.localScale = vec;


		level.Add (Instantiate (start, start.transform.position, start.transform.rotation));
		int i = 10;
		int rand = rnd.Next (patterns.Length);
		while (level.Count < 3) {
			vec = patterns[rand].transform.position;
			vec.x += i;
			tmp = Instantiate (patterns[rand], vec, patterns[rand].transform.rotation);
			GameObject star = tmp.transform.Find ("star").gameObject;
			Destroy (star);
			level.Add (tmp);
			i += 15;
			last = rand;
			rand = rnd.Next (patterns.Length);
			if (last == rand) {
				rand = (last + 1) % patterns.Length;
			}
		}
		scoreTxt.text = "Score: " + score;
    }


    private void Update()
    {
		if (!GameManager.GM.pause) {
			generate ();
			if (isStart) {
				if (score % 1000 == 0 && score < 10000) {
					scrollSpeed += 0.005f;
				}
				if (!die) {
					++score;
					scoreTxt.text = "Score: " + score;
					scrollScreen ();
				}
				if (!spike) {
					Vector3 vec = spikes.transform.position;
					vec.x += 0.03f;
					spikes.transform.position = vec;
					if (vec.x >= -1.0f) {
						spike = true;
					}
				}
			}
			moveAndAnimation ();
			if (bonus > 0) {
				Vector3 tmp = starIcon.transform.localScale;
				tmp.x = 8f * bonus / 1000f;
				tmp.y = 7f * bonus / 1000f;
				starIcon.transform.localScale = tmp;
				--bonus;
			}
		}
    }

	private void moveAndAnimation()
	{
		if (!die) {
			Flip ();
			if (Input.GetKey (GameManager.GM.right)) {
				if (vh < 1.0f) {
					vh += 0.25f;
				}
			} else if (Input.GetKey (GameManager.GM.left)) {
				if (vh > -1.0f) {
					vh -= 0.25f;
				}
			}
			if (Input.GetKey(GameManager.GM.left) || Input.GetKey(GameManager.GM.right)) {
				if (!isStart) {
					isStart	= true;
				}
				if (allowJump)
					anim.SetInteger ("State", 1);
			} else {
				vh = 0;
				if (allowJump)
					anim.SetInteger ("State", 0);
			}
			if (Input.GetKey (GameManager.GM.jump) && allowJump) {
				vg = 10;
				anim.SetInteger ("State", 2);
			}
			vg -= gravity;
			player.velocity = new Vector2 (vh * speed, vg);
			if (player.position.y <= -10) {
				anim.SetInteger ("State", 3);
				die = true;
				FindObjectOfType<AudioManager> ().Stop ("GameSound");
				FindObjectOfType<AudioManager> ().Play ("PlayerDead");
				FindObjectOfType<ScoresManager> ().InsertScore (score);
			}
		} else if (tempoDie < 110) {
			vg -= gravity;
			player.velocity = new Vector2 (0, vg);
			++tempoDie;
			if (tempoDie == 110) {
				scoreTxt.gameObject.SetActive (false);
				scoreGameOver.text = score.ToString ();
				scoreGameOver.gameObject.SetActive (true);
				gameover.SetActive (true);
				FindObjectOfType<AudioManager> ().Play ("GameOver");
			}
		} else if (Input.GetKey (KeyCode.Escape)) {
			score = 0;
			scoreTxt.gameObject.SetActive (true);
			scoreGameOver.gameObject.SetActive (false);
			gameover.SetActive (false);
			FindObjectOfType<AudioManager> ().Stop ("GameOver");
			FindObjectOfType<AudioManager> ().Play ("GameSound");

			vh = 0;
			anim.SetInteger ("State", 0);

			while (level.Count != 0) {
				Destroy (level [0]);
				level.RemoveAt (0);
			}
			while (explodes.Count != 0) {
				Destroy (explodes [0]);
				explodes.RemoveAt (0);
			}
			spikes.transform.position = spikeX;
			player.transform.position = playerX;
			spike = false;
			die = false;
			isStart = false;
			allowJump = false;
			bonus = 0;
			scrollSpeed = 0.025f;

			if (!facingRight) {
				facingRight = !facingRight;
				Vector3 tmpScale = transform.localScale;
				tmpScale.x *= -1;
				transform.localScale = tmpScale;
			}

			GameObject tmp;
			Vector3 vec = starIcon.transform.localScale;
			vec.x = 0;
			vec.y = 0;
			starIcon.transform.localScale = vec;


			level.Add (Instantiate (start, start.transform.position, start.transform.rotation));
			int i = 10;
			int rand = rnd.Next (patterns.Length);
			while (level.Count < 3) {
				vec = patterns[rand].transform.position;
				vec.x += i;
				tmp = Instantiate (patterns[rand], vec, patterns[rand].transform.rotation);
				GameObject star = tmp.transform.Find ("star").gameObject;
				Destroy (star);
				level.Add (tmp);
				i += 15;
				last = rand;
				rand = rnd.Next (patterns.Length);
				if (last == rand) {
					rand = (last + 1) % patterns.Length;
				}
			}
			scoreTxt.text = "Score: " + score;
			tempoDie = 0;
		}
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
		foreach (GameObject obj in explodes)
		{
			Vector3 test = obj.transform.position;
			test.x -= scrollSpeed;
			obj.transform.position = test;
		}
	}

	private void generate()
	{
		if (level [0].transform.position.x <= -25f) {
			Destroy (level [0]);
			level.RemoveAt (0);
			int tmpRnd = rnd.Next (patterns.Length);
			if (tmpRnd == last) {
				tmpRnd = (last + 1) % patterns.Length;
			}
			last = tmpRnd;
			Vector3 vec = level [level.Count - 1].transform.position;
			vec.x += 15;
			vec.y = patterns [tmpRnd].transform.position.y;
			GameObject tmp = Instantiate(patterns [tmpRnd], vec, level [level.Count - 1].transform.rotation);
			if (rnd.Next (3) != 0) {
				GameObject star = tmp.transform.Find ("star").gameObject;
				Destroy (star);
			}
			level.Add (tmp);
		}
		if (explodes.Count != 0 && explodes [0].transform.position.x <= -25f) {
			Destroy (explodes [0]);
			explodes.RemoveAt (0);
		}
	}

	private void Flip()
	{
		if ((Input.GetKey(GameManager.GM.right) && !facingRight) || (Input.GetKey(GameManager.GM.left) && facingRight)) {
			facingRight = !facingRight;
			Vector3 tmp = transform.localScale;
			tmp.x *= -1;
			transform.localScale = tmp;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.CompareTag ("Ground")) {
			vg = 0;
			allowJump = true;
		} else if (coll.gameObject.CompareTag ("Wall")) {
			allowJump = true;
		} else if (coll.gameObject.CompareTag ("Ceil")) {
			vg = 0;
		} else if (coll.gameObject.CompareTag ("Obstacle")) {
			if (bonus > 0) {
				Vector3 vec = coll.gameObject.transform.position;
				vec.y += 0.5f;
				if (coll.gameObject.name == "tortle") {
					explodes.Add (Instantiate (explode, vec, coll.gameObject.transform.rotation));
				} else {
					explodes.Add (Instantiate (explode_scie, vec, coll.gameObject.transform.rotation));
				}
				FindObjectOfType<AudioManager> ().Play ("Explosion");
				Destroy (coll.gameObject);
			} else if (!die) {
				anim.SetInteger ("State", 3);
				die = true;
				FindObjectOfType<AudioManager> ().Stop ("GameSound");
				FindObjectOfType<AudioManager> ().Play ("PlayerDead");
				FindObjectOfType<ScoresManager> ().InsertScore (score);
			}
		} else if (coll.gameObject.CompareTag ("Star")) {
			bonus = 1000;
			FindObjectOfType<AudioManager> ().Play ("Bonus");
			Destroy (coll.gameObject);
		} else if (!die && coll.gameObject.CompareTag ("Death")) {
			anim.SetInteger ("State", 3);
			die = true;
			FindObjectOfType<AudioManager> ().Stop ("GameSound");
			FindObjectOfType<AudioManager> ().Play ("PlayerDead");
			FindObjectOfType<ScoresManager> ().InsertScore (score);
		}
	}

	void OnCollisionStay2D(Collision2D coll)
	{
		if (coll.gameObject.CompareTag ("Ground")) {
			vg = 0;
			allowJump = true;
		} else if (coll.gameObject.CompareTag ("Wall")) {
			allowJump = true;
		}
	}
}
