using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestButtonScript : MonoBehaviour {
	public Button thisButton;
	public Player thePlayer = null;

	// Use this for initialization
	void Start () {
		thisButton = GameObject.Find ("Button").GetComponent<Button> ();
		thePlayer = GameObject.Find ("player(Clone)").GetComponent<Player> ();

	}

	void Update() {
		//if (thePlayer == null && GameObject.Find ("player") != null)  {
			
		//}
	}

	// Update is called once per frame
	void OnMouseEnter () {
		thePlayer.buttonMouseOver = true;
	}

	void OnMouseExit () {
		thePlayer.buttonMouseOver = false;
	}
}
