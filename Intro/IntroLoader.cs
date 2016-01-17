using UnityEngine;
using System.Collections;

public class IntroLoader : MonoBehaviour {

	public GameObject introManager;

	void Awake () {
		if (IntroManager.instance == null) {
			introManager = new GameObject ("IntroManager");
			introManager.AddComponent<IntroManager> ();
		}
	}
}
