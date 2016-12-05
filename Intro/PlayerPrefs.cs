using UnityEngine;
using System.Collections;

public class PlayerPrefs : MonoBehaviour {

	public bool customizeBaseOnStart;
	public string leaderName;
	public bool tutorialOn;
	public bool boldonVO;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this);
	}
}
