using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {

	public static IntroManager instance = null;

	public Button nextButton;
	public Toggle customizeToggle;
	public Toggle tutorialToggle;
	public Toggle devNotesToggle;
	public InputField leaderName;
	public GameObject optionPanel;
	public Text titleText;
	public Text bodyText;
	public GameObject playerPrefsGO;
	public PlayerPrefs playerPrefs;

	private int pageState = 0;
	private int devNotesEndPage = 2;
	private int tutorialEndPage = 3;


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
		devNotesToggle = GameObject.Find ("DevNotesToggle").GetComponent<Toggle> ();
		leaderName = GameObject.Find ("ResearcherNameInputField").GetComponent<InputField> ();
		optionPanel = GameObject.Find ("GameOptionsPanel");
		titleText = GameObject.Find ("TitleText").GetComponent<Text> ();
		bodyText = GameObject.Find ("BodyText").GetComponent<Text> ();

		nextButton.onClick.AddListener (() => {
			NextScreen();
		});

		bodyText.enabled = false;
		titleText.text = "Thought Crimes Division (v0.2)";
	}
	
	public void NextScreen() {
		if (pageState == 0) {
			if (leaderName.text == "")
				playerPrefs.leaderName = "Valerie";
			else if (leaderName.text.Length >= 22)
				playerPrefs.leaderName = "Troller McTrollface";
			else
				playerPrefs.leaderName = leaderName.text;
			playerPrefs.customizeBaseOnStart = customizeToggle.isOn;
			playerPrefs.tutorialOn = tutorialToggle.isOn;

			if (!devNotesToggle.isOn) {
				pageState = devNotesEndPage;
			}
				

//			pageState++;
//			titleText.text = "(Bad) Tutorial";
//			bodyText.text = "Hi this is some tutorial text. Lah di dah";
//			optionPanel.SetActive (false);
//			bodyText.enabled = true;
//			//Toggle stuff on/off
		} 

		if ((pageState == devNotesEndPage || !devNotesToggle.isOn) && !tutorialToggle.isOn) {
			StartGame ();
		} else if (optionPanel.activeInHierarchy) {
			optionPanel.SetActive (false);
			bodyText.enabled = true;
		}

		pageState++;



		if (pageState == 1) {
			//GO TO DEV NOTES
			titleText.text = "Dev Notes";
			bodyText.text = "Hi,\nThis is my first game - its mainly an experiment to learn Unity and see how far I can get making something like this by myself. I originally started with the idea of recreating an old favorite of mine - Evil Genius, but I also drew a lot of inspiration from the turn-based tactics games and recent roguelike strategy games (XCOM, Fire Emblem, Dungeon of the Endless, Invisible Inc). The grand vision involves a story mode with iterative base-defense loops, a more comprehensive system for espionage and research (not just combat), and the ability to manipulate the world’s political structure using mind control (think Shadows of Mordor-esque power structure mechanics), but those dreams are a long ways off. I’m not sure I’ll continue developing this past this early release (instead pivoting onto other game ideas), but if I’m filled with enough determination, I’ll find time to polish this experience. I hope you enjoy - if you have feedback, please send it my way at synthesse@gmail.com.\nglhf!\n-Naomi\n\n\nArt/Music Credits\n\t- charas-project.net\n\t- Daniel Cook/Hard Vacuum\n\t- Kenney.nl\n\t- Sonniss GDC Bundle\n\t- Abstraction (www.abstractionmusic.com)";
		} else if (pageState == 2) {
			//DEV NOTES - TECHNICAL
			bodyText.text = "Features Slated for Next Release\n\t- More units types (Spy, Hacker, Researcher, Infiltrator, et al)\n\t- More defense types\n\t- More complicated abilities (AoE targeting, overwatch, traps, Status effects, Cooldowns)\n\t- Espionage system\n\t- Research system\n\t- Better art\n\t- Better unit AI\n\t- Actual Menus\n\t- Data persistence (Save, Load, High Score)\n\t- Cover Mechanics\n\t- Balance\n\nKnown Bugs\n\t- Build ghost sprite will sometimes clip through map grid barriers\n\t- Objects with larger sprites will behave weirdly targeted/moved to by enemy AI\n\t- Allied units block line of sight of ally-targeted abilities\n\t- AI Melee attacks will rarely extend outside melee range\n\nChangelog\n\tFirst Release";
		} else if (pageState == 3) {
			titleText.text = "Tutorial";
			bodyText.text = "Thought Crimes Division is a turn-based strategy game where you must build a base and defend it from waves of enemies - think XCOM meets Dungeon Keeper. You play as "+playerPrefs.leaderName+", leader of an experimental combat research group, and have recently invented a mind control device. You must use your advanced technology to hold out as long as you can against the endless onslaught of enemies.\n\nDuring play, you will alternate between a build phase and a combat phase. During the build phase, you can move your units freely, build defenses with available cash, and excavate new areas. During the combat phase, your units have limited action points they can use to move, use abilities, or attack enemies within line of sight. You and your enemies will take turns, dealing damage to each others’ units. If you take out all enemies, the combat phase is over - your units heal slightly and you receive cash; if your leader is killed, the game is over.\n\nControls\n\tLeftClick = Select/Target/Confirm\n\tRightClick = Cancel/Automove\n\tesc = Quit\n\tm = Mute\n\t1-3 = Combat Abilities\n\tSpace = Recenter Camera\n\nTips\n\tYou can rightclick to select an object underneath another\n\tClick the blinking objects for the tutorial.";
		} else {
			StartGame ();
		}
	}

	void Update() {
		if (tutorialToggle.isOn && customizeToggle.interactable) {
			customizeToggle.interactable = false;
			customizeToggle.isOn = false;
		} else if (!tutorialToggle.isOn && !customizeToggle.interactable) {
			customizeToggle.interactable = true;
		}
	}

	public void StartGame() {
		SceneManager.LoadScene ("Main");
	}
}
