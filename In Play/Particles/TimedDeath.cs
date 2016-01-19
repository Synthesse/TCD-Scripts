using UnityEngine;
using System.Collections;

public class TimedDeath : MonoBehaviour {

	public float deathTime;
	private float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > (startTime+deathTime))
			Destroy(gameObject);
	}
}
