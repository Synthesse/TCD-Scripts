using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	public int tutorialStage;
	public int tutorialSubStage;
	private GameManager gameManager;
	public Sprite leaderSprite;
	public Sprite turretSprite;
	public Sprite neuralAmpSprite;
	public Sprite soldierSprite;
	public Sprite[] buildSprites;
	public Sprite assassinSprite;
	public Sprite shielderSprite;
	public Sprite robotSprite;
	private bool waiting;

	void Awake() {
		tutorialStage = -1;
	}

	void Start() {
		tutorialStage++;
		tutorialSubStage = 0;
	}

	public void Initialize() {
		gameManager = GameManager.instance;
		gameManager.uiManager.buildToggle.interactable = false;
		gameManager.uiManager.startWaveButton.interactable = false;
		StartCoroutine (StartBlinking (FindObjectOfType<Leader>().gameObject));
	}

	private IEnumerator StartBlinking(GameObject objectToBlink) {
		int initTutorialStage = tutorialStage;
		Color tutorialStoredColor;
		Color redColor = new Color (1, 0, 0);
		bool blink = false;

		if (objectToBlink.GetComponentInChildren<Image> () != null) {
			Image objImage = objectToBlink.GetComponentInChildren<Image> ();
			tutorialStoredColor = objImage.color;
			while (initTutorialStage == tutorialStage) {
				if (blink) {
					objImage.color = tutorialStoredColor;
					blink = false;
				} else {
					objImage.color = redColor;
					blink = true;
				}
				yield return new WaitForSeconds (0.5f);
			}
			objImage.color = new Color (1, 1, 1);
		} else {
			SpriteRenderer objSpriteRend = objectToBlink.GetComponent<SpriteRenderer> ();
			tutorialStoredColor = objSpriteRend.color;
			while (initTutorialStage == tutorialStage) {
				if (blink) {
					Debug.Log ("COLOR RETURN");
					objSpriteRend.color = tutorialStoredColor;
					blink = false;
				} else {
					Debug.Log ("COLOR RED");
					objSpriteRend.color = redColor;
					blink = true;
				}
				yield return new WaitForSeconds (0.5f);
			}
			objSpriteRend.color = new Color (1, 1, 1);
		}
	}

	private void EnableTutorial() {
		Debug.Log ("TUTORIAL LEVEL: " + tutorialStage.ToString());
		gameManager.playerInput.TogglePlayerInputLock (true);
		gameManager.DeselectObject ();
		if (tutorialStage == 0) {
			gameManager.uiManager.EnableTutorialScreen (leaderSprite, "Tutorial: Lead Researcher", "This is the Lead Researcher of the Thought Crimes Division and your primary unit. If she dies, the game is over. She has three abilities:\n\t-She can shoot her laser pistol at short range for low damage\n\t-She can haste an allied unit, giving it bonus action points\n\t-She can permanently dominate the mind of another unit\n\nKeep in mind, enemies can only be mind controlled if they are below half HP and if a neural amplifier has been built and is unused.\nOutside of combat, you can freely move your lead researcher around the base by selecting them, then right clicking your destination. In combat, this will cost action points (AP).");
		} else if (tutorialStage == 1) {
			gameManager.uiManager.EnableTutorialScreen (turretSprite, "Tutorial: Defenses", "In your base, you can construct defenses to assist your leader. The defense you selected is a Turret, a static gun which fires long-range laser bolts.");
		} else if (tutorialStage == 2) {
			gameManager.uiManager.EnableTutorialScreen (neuralAmpSprite, "Tutorial: Neural Amplifier", "Another machine you can build is a Neural Amplifier. Neural Amps act as relays, allowing you to control minds. However, you need one neural amp for each enemy you want to control. If this device is destroyed, the neural feedback will kill the unit it is helping control.");
		} else if (tutorialStage == 3) {
			gameManager.uiManager.EnableTutorialScreen (soldierSprite, "Tutorial - Combat", "You are now starting combat. To leave combat, you must kill or dominate all enemies. For a unit to attack or use abilities, it must have enough action points, be in range, and have clear line of sight of its target. Keep in mind that many objects will block line of sight and pathing.\n\nThe first enemy you'll face is the Solider - he has no glaring weaknesses and wields a long range laser rifle.");
		} else if (tutorialStage == 4 && tutorialSubStage == 0) {
			gameManager.uiManager.EnableTutorialScreen (buildSprites[0], "Tutorial - Build Mode", "In build mode, you can use the cash you acquire from combat to improve your base. The first option in build mode, Remove Walls, helps craft the layout of your base. You can use this to create new passageways, doorways and rooms. Once you remove wall, you cannot replace it - plan ahead.\n\nNow we'll cover the different objects you can build. We've already gone over the Turret and Neural Amplifier, so we'll run through the remaining three.");
		} else if (tutorialStage == 5 && tutorialSubStage == 1) {
			gameManager.uiManager.EnableTutorialScreen (buildSprites[1], "Tutorial - Build Mode", "The Shield Door takes up two squares and can be rotated. It has two states, active and inactive. By default, it can be freely walked over. During combat, this can be activated to project an energy shield, blocking off passageways and providing cover.");
		} else if (tutorialStage == 5 && tutorialSubStage == 2) {
			gameManager.uiManager.EnableTutorialScreen (buildSprites[2], "Tutorial - Build Mode", "The Research Machine is large object, occupying 4 squares. It would normally enable you to do science, but in this super early build, it just increases your cash income per wave.");
		} else if (tutorialStage == 5 && tutorialSubStage == 3) {
			gameManager.uiManager.EnableTutorialScreen (buildSprites[3], "Tutorial - Build Mode", "The Remote Mine can be walked over freely and is invisible to enemies. During combat, it can be triggered remotely to deal massive damage in a 3x3 area.");
			tutorialSubStage++;
		} else if (tutorialStage == 5) {
			gameManager.uiManager.EnableTutorialScreen (assassinSprite, "Tutorial - Assassin", "The Assassin is a fragile melee unit which can strike rapidly for high damage. He can also teleport long distances to opponents within line of sight.");
		} else if (tutorialStage == 6) {
			gameManager.uiManager.EnableTutorialScreen (shielderSprite, "Tutorial - Captain", "The Captain is a sturdy, supportive combatant. She wields a short range laser pistol and an energy shield, which blocks most of the damage coming from the direction she's facing. She can also mark targets from range, increasing the damage they take.");
		} else if (tutorialStage == 7) {
			gameManager.uiManager.EnableTutorialScreen (robotSprite, "Tutorial - Murderbot", "The Murderbot is relentless and destructive. It cannot be mind controlled and attacks by electrocuting enemies, dealing more damage to machines. When it dies, it explodes dealing tons of damage to everything in melee range.\n\nThis ends the tutorial. Good luck, have fun!");
		} 
	}

	public void TransitionFromTutorialScreen() {
		if (tutorialStage == 5 && tutorialSubStage <= 2) {
			Debug.Log ("ENTER SUBSTAGE");
			tutorialSubStage++;
			gameManager.playerInput.TogglePlayerInputLock (false);
			EnableTutorial ();
		} else {
			gameManager.playerInput.TogglePlayerInputLock (false);
			gameManager.uiManager.DisableTutorialScreen ();
		}
	}

	private IEnumerator WaitUntilEndOfCombat() {
		waiting = true;
		yield return new WaitWhile (() => gameManager.combatManager.combatModeEnabled);
		waiting = false;
		tutorialStage++;
		if (tutorialStage == 4) {
			gameManager.uiManager.buildToggle.interactable = true;
			gameManager.uiManager.startWaveButton.interactable = false;
			StartCoroutine (StartBlinking (gameManager.uiManager.buildToggle.gameObject));
		}
	}

	void Update() {
		if (tutorialStage == 0 && gameManager.selectedObject != null && gameManager.selectedObject.GetComponent<Leader> () != null) {
			EnableTutorial ();
			tutorialStage++;
			StartCoroutine (StartBlinking (FindObjectOfType<Turret> ().gameObject));
		} else if (tutorialStage == 1 && gameManager.selectedObject != null && gameManager.selectedObject.GetComponent<Turret> () != null) {
			EnableTutorial ();
			tutorialStage++;
			StartCoroutine (StartBlinking (FindObjectOfType<NeuralAmplifier> ().gameObject));
		} else if (tutorialStage == 2 && gameManager.selectedObject != null && gameManager.selectedObject.GetComponent<NeuralAmplifier> () != null) {
			EnableTutorial ();
			tutorialStage++;
			gameManager.uiManager.startWaveButton.interactable = true;
			StartCoroutine (StartBlinking (gameManager.uiManager.startWaveButton.gameObject));
		} else if (tutorialStage == 3 && !waiting && gameManager.combatManager.combatModeEnabled) {
			EnableTutorial ();
			StartCoroutine (WaitUntilEndOfCombat ());
		} else if (tutorialStage == 4 && gameManager.uiManager.buildToggle.isOn) {
			EnableTutorial ();
			gameManager.uiManager.startWaveButton.interactable = true;
			tutorialStage++;
		} else if (tutorialStage >= 5 && tutorialStage < 8 && !waiting && gameManager.combatManager.combatModeEnabled) {
			EnableTutorial ();
			StartCoroutine (WaitUntilEndOfCombat ());
		} else if (tutorialStage == 8) {
			this.enabled = false;
		}
	}
}

/*Stages
 * 0 = start
 * 1 = introduced player
 * 2 = introduced turret
 * 3 = introduced neural amplifier
 * 4 = introduced combat/soldier
 * 5 = Introduced build options
 * 6 = introduced assassin
 * 7 = introduced captain
 * 8 = introduced murderbot (finished)
*/