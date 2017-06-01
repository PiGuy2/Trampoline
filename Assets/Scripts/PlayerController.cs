using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerController : MonoBehaviour {

	// Manual
	public bool continuousMove = true;
	public bool stopAllowed = false;
	public float divPerCycle = 1.03f;
	public float continuousDiv = 50;
	public bool jumpAllowed = false;
	public float speedMult = 10;
	public float jumpMult = 6;
	public float jumpMoveMult = 500;
	public float allMoveMult = 1;

	// Script
	//

	// private
	private Rigidbody rb;
	private Transform tr;
	private bool doJump = false;
	private int jumping = 0;
	private float h,v = 0;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		tr = GetComponent<Transform>();
	}

	private float t;
	private int f;
	private Vector3 zer = new Vector3(0, 0, 0);

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 move;
		float horizontalMove = Input.GetAxis("Horizontal");
		float verticalMove = Input.GetAxis("Vertical");

		if (Math.Abs(verticalMove) > 0 && Math.Abs(horizontalMove) > 0) {
			verticalMove /= 2;
			horizontalMove /= 2;
		}

		if (jumping == 0) {
			// rolling
			move = new Vector3(horizontalMove, 0f, verticalMove);
			move *= speedMult;
			if (jumpAllowed) {
				if (Input.GetKey(KeyCode.Space)) {
					h = horizontalMove * jumpMoveMult;
					v = verticalMove * jumpMoveMult;
					if (continuousMove) {
						move = new Vector3(h / continuousDiv, 100 * jumpMult, v / continuousDiv);
					} else {
						move = new Vector3(h, 100 * jumpMult, v);
					}
					jumping = 1;
				}
			}
		} else if (jumping == 1) {
			// Manual jump, not touching ground
			if (continuousMove) {
				if (stopAllowed) {
					if (Math.Abs(Input.GetAxis("Horizontal")) == 0) {
						h = 0;
					}
					if (Math.Abs(Input.GetAxis("Vertical")) == 0) {
						v = 0;
					}
				}
				h /= divPerCycle;
				v /= divPerCycle;
				move = new Vector3(h / continuousDiv, 0f, v / continuousDiv);
			} else {
				move = zer;
			}
		} else if (jumping == 2) {
			// jumping
			if (doJump) {
				print(Time.realtimeSinceStartup - t);
				print(f);
				t = Time.realtimeSinceStartup;
				f = 0;
				//
				h = horizontalMove * jumpMoveMult;
				v = verticalMove * jumpMoveMult;
				if (continuousMove) {
					move = new Vector3(h / continuousDiv, 100 * jumpMult, v / continuousDiv);
				} else {
					move = new Vector3(h, 100 * jumpMult, v);
				}
				doJump = false;
			} else {
				if (continuousMove) {
					if (stopAllowed) {
						if (Math.Abs(Input.GetAxis("Horizontal")) == 0) {
							h = 0;
						}
						if (Math.Abs(Input.GetAxis("Vertical")) == 0) {
							v = 0;
						}
					}
					h /= divPerCycle;
					v /= divPerCycle;
					move = new Vector3(h / continuousDiv, 0f, v / continuousDiv);
				} else {
					move = zer;
				}
			}
		} else {
			Debug.LogWarning("Variable \"Jumping\" cannot be " + jumping.ToString());
			move = zer;
		}
		rb.AddForce(move * allMoveMult);
		f++;
	}

	void OnTriggerEnter (Collider other) {
		GameObject o = other.gameObject;
		if (o.CompareTag("Jump")) {
//			print("Ground");
			if (tr.position.y > o.transform.position.y) {
				jumping = 2;
				doJump = true;
			} else {
				print("under");
			}
		} else if (o.CompareTag("NoJump")) {
//			print("Ground");
			jumping = 0;
			doJump = false;
		}
	}
}