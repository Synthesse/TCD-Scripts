using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {

	public static IntroManager instance = null;

	public Button nextButton;
	public Toggle customizeToggle;
	public Toggle tutorialToggle;
	public InputField leaderName;
	public GameObject optionPanel;
	public Text titleText;
	public Text bodyText;
	public GameObject playerPrefsGO;
	public PlayerPrefs playerPrefs;

	private int pageState = 0;

	// Use this for initialization
	void Start () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		
		playerPrefsGO = new GameObject ("PlayerPrefs");
		playerPrefsGO.AddComponent<PlayerPrefs> ();
		playerPrefs = playerPrefsGO.GetComponent<PlayerPrefs> ();

		nextButton = GameObject.Find ("NextButton").GetComponent<Button> ();
		customizeToggle = GameObject.Find ("CustomizeBaseToggle").GetComponent<Toggle> ();
		tutorialToggle = GameObject.Find ("TutorialToggle").GetComponent<Toggle> ();
		leaderName = GameObject.Find ("ResearcherNameInputField").GetComponent<InputField> ();
		optionPanel = GameObject.Find ("GameOptionsPanel");
		titleText = GameObject.Find ("TitleText").GetComponent<Text> ();
		bodyText = GameObject.Find ("BodyText").GetComponent<Text> ();

		nextButton.onClick.AddListener (() => {
			NextScreen();
		});

		optionPanel.SetActive (false);
		titleText.text = "Note to Playtesters";
		bodyText.text = "This is a note for the playtesters. WIP";
	}
	
	public void NextScreen() {
		if (pageState == 0) {
			pageState++;
			titleText.text = "Game Options";
			bodyText.enabled = false;
			optionPanel.SetActive (true);

		} else if (pageState == 1) {
			if (leaderName.text == "")
				playerPrefs.leaderName = "Valerie";
			else
				playerPrefs.leaderName = leaderName.text;
			playerPrefs.customizeBaseOnStart = customizeToggle.isOn;
			if (!tutorialToggle.isOn)
				StartGame ();
			else {
				pageState++;
				titleText.text = "(Bad) Tutorial";
				bodyText.text = "Hi this is some tutorial text. Lah di dah";
				optionPanel.SetActive (false);
				bodyText.enabled = true;
				//Toggle stuff on/off
			}
		} else {
			StartGame ();
		}
	}

	public void StartGame() {
		SceneManager.LoadScene ("Main");
	}
}
