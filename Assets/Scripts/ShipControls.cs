﻿using UnityEngine;
using System.Collections;
using System;

public class ShipControls : MonoBehaviour {

	private const float ANGLETORADIANS = 0.0174532925f;

	private Vector2 movement;
	private Vector3 rotation;

	private float angleSpeed=0;
	//private int cyclesSinceLastBurst = 100;

	private ThrusterAnimation thrusterAnimation;
	private AudioSource shipAudioSource;

	public float thrust = 0.35f;
	public float maxAngleSpeed = 150f;

	public float shipAccel { get; set;}

	public delegate void ChangedEventHandler(object sender, EventArgs e);
	public event ChangedEventHandler ShipActivated;

	void Start () {
		GameState.Instance.playerActive = true;
		shipAccel = 0;
		thrusterAnimation = GameObject.FindGameObjectWithTag ("Thruster").GetComponent<ThrusterAnimation> ();
		shipAudioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameState.Instance.playerActive){
			calculateAcceleration ();

			if(Input.GetButtonDown("Thrust")){
				thrusterAnimation.startThrusting();
			}
			if(Input.GetButtonUp("Thrust")){
				thrusterAnimation.stopThrusting();
			}

			if(Input.GetKeyDown ("x")){
				flip();
			}
		}
	}

	private void calculateAcceleration(){
		//cyclesSinceLastBurst++;
		if(Input.GetButton ("Thrust")){
			//float xThrust = this.thrust*Mathf.Cos(transform.rotation.eulerAngles.z*ANGLETORADIANS);
			if(flipped){
				shipAccel = -this.thrust;
			}
			else{
				this.shipAccel = this.thrust;
			}
			if(rigidbody2D.velocity.magnitude < 6){
				this.shipAccel*=2;
			}
			/*
			 * 
			 * If player hasn't thrusted in a few cycles give boost
			if(this.cyclesSinceLastBurst>100){
				this.shipAccel*=40;
				this.cyclesSinceLastBurst=0;
			}
			else{
				this.cyclesSinceLastBurst/=2;
			}*/
		}
		else{
			shipAccel = 0;
		}
	}

	void FixedUpdate ()
	{
		if(GameState.Instance.playerActive){

			applyAcceleration ();

			float hArrowKeyInput = Input.GetAxis("Horizontal");
			if(hArrowKeyInput > 0){
				angleSpeed = -maxAngleSpeed;
				rigidbody2D.angularVelocity = angleSpeed;
			}
			else if(hArrowKeyInput < 0){
				angleSpeed = maxAngleSpeed;
				rigidbody2D.angularVelocity = angleSpeed;
			}
			checkForNeedToFlip ();
		}
	}

	public void deactivateShip(){
		GameState.Instance.playerActive = false;
		GetComponent<SpriteRenderer> ().enabled = false;
		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		GetComponent<Rigidbody2D> ().angularVelocity = 0f;
		GameState.Instance.exitOpenSpace();
		thrusterAnimation.stopThrusting ();
		if(flipped){
			flip();
		}
	}

	public void activateShip(){
		GameState.Instance.playerActive = true;
		GetComponent<SpriteRenderer> ().enabled = true;
		GameState.Instance.exitOpenSpace();
		checkThrustingSound ();
		if(ShipActivated != null){
			ShipActivated(this, EventArgs.Empty);
		}
	}

	public void checkThrustingSound(){
		if (Input.GetButton ("Thrust")) {
			thrusterAnimation.startThrusting();
		}
	}

	public void moveToLastCheckpoint(){
		CheckPoint lastPoint = GameState.Instance.getLastCheckPoint();
		Vector3 checkPointPosition = lastPoint.transform.position;
		transform.position = new Vector3 (checkPointPosition.x, checkPointPosition.y, transform.position.z);
		rigidbody2D.angularVelocity = 0f;
		transform.rotation = lastPoint.transform.rotation;
		transform.Rotate (0,0,90);
	}

	public void die(){
		deactivateShip ();
		shipAudioSource.Play ();
	}

	private void applyAcceleration(){
		//rigidbody2D.velocity += thrustVector;
		rigidbody2D.AddForce (transform.right*shipAccel);

		if(!GameState.Instance.InOpenSpace){
			rigidbody2D.velocity*=0.98f;
		}

	}

	private void checkForNeedToFlip(){
		float currentAngle = transform.rotation.eulerAngles.z;
		if(currentAngle > 180){
			currentAngle = currentAngle-360;
		}
		if(currentAngle <-90){
			flip();
			transform.Rotate (new Vector3(0,0,180));
		}
		else if(currentAngle >90){
			flip();
			transform.Rotate (new Vector3(0,0,180));
		}
	}

	private void flip(){
		transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,0);
	}

	private bool flipped{
		get{
			return transform.localScale.x < 0; 
		}

	}

}