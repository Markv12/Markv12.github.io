﻿using UnityEngine;
using System.Collections;

public class GoalPortal : MonoBehaviour {

	public string levelName;

	private IEnumerator WaitThenExit(float seconds) {
		yield return new WaitForSeconds(seconds);
		GameState.Instance.LoadLevel(levelName);
	}


	void OnTriggerEnter2D(Collider2D collider)
	{
		ShipControls ship = collider.gameObject.GetComponent<ShipControls>();
		if (ship != null && GameState.Instance.isLevelStarted)
		{
			ship.deactivateShip();
			StartCoroutine(WaitThenExit(1.5f));

		}
	}
}