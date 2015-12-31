using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameManager gameManager;
	private int storedNumCombatActions;

	public Button startWaveButton;
	public Button switchBuildMenusButton;
	public Button nextTurnButton;
	public Toggle buildToggle;
	public Toggle detailToggle;
	public Text waveNumText;
	public Text turnStatusText;
	public Text cashText;
	public Text selectedUnitText;
	public Text vitalStatsText;
	public Text buildNoteText;

	public GameObject detailPanel;
	public GameObject buildPanel;
	public GameObject combatPanel;
	public Button[] buildPanelButtons;
	public Button[] combatPanelButtons;
	public Text detailPanelText;

	public LineRenderer pathRenderLine;


	// Use this for initialization
	public void Initialize () {
		gameManager = GameManager.instance;

		startWaveButton = GameObject.Find ("startWaveButton").GetComponent<Button> ();
		switchBuildMenusButton = GameObject.Find ("switchBuildMenusButton").GetComponent<Button> ();
		nextTurnButton = GameObject.Find ("nextTurnButton").GetComponent<Button> ();
		buildToggle = GameObject.Find ("buildToggle").GetComponent<Toggle> ();
		detailToggle = GameObject.Find ("detailToggle").GetComponent<Toggle> ();
		waveNumText = GameObject.Find ("waveNumText").GetComponent<Text> ();
		turnStatusText = GameObject.Find ("turnStatusText").GetComponent<Text> ();
		cashText = GameObject.Find ("cashText").GetComponent<Text> ();
		selectedUnitText = GameObject.Find ("selectedUnitText").GetComponent<Text> ();
		vitalStatsText = GameObject.Find ("vitalStatsText").GetComponent<Text> ();
		buildNoteText = GameObject.Find ("buildNoteText").GetComponent<Text> ();

		detailPanel = GameObject.Find ("detailPanel");
		buildPanel = GameObject.Find ("buildPanel");
		combatPanel = GameObject.Find ("combatPanel");
		buildPanelButtons = buildPanel.GetComponentsInChildren<Button> ();
		combatPanelButtons = combatPanel.GetComponentsInChildren<Button> ();
		detailPanelText = detailPanel.GetComponentInChildren<Text> ();

		pathRenderLine = GameObject.Find ("Pathfinding Renderer").GetComponent<LineRenderer> ();

		//Initialize Start State
		buildPanel.SetActive(false);
		combatPanel.SetActive(false);
		detailPanel.SetActive (false);
		detailToggle.gameObject.SetActive (false);
		switchBuildMenusButton.gameObject.SetActive (false);
		nextTurnButton.gameObject.SetActive (false);
		waveNumText.enabled = false;
		turnStatusText.enabled = false;
		selectedUnitText.enabled = false;
		vitalStatsText.enabled = false;
		buildNoteText.enabled = false;
		pathRenderLine.enabled = false;

	}

	public void ToggleSelectedUnitUI(bool turnOn) {
		if (turnOn) {
			selectedUnitText.enabled = true;
			detailToggle.gameObject.SetActive (true);
			vitalStatsText.enabled = true;
			if (gameManager.combatManager.combatModeEnabled && gameManager.selectedObject.tag == "Player") {
				combatPanel.SetActive (true);
			}
		} else {
			selectedUnitText.enabled = false;
			detailToggle.isOn = false;
			detailToggle.gameObject.SetActive (false);
			vitalStatsText.enabled = false;
			if (gameManager.combatManager.combatModeEnabled) {
				combatPanel.SetActive (false);
			}
		}
	}

	public void ToggleSelectedUnitUI(bool turnOn, int numCombatActions) {
		if (gameManager.combatManager.combatModeEnabled && gameManager.selectedObject.tag == "Player") {
			storedNumCombatActions = numCombatActions;
			ToggleCombatPanelButtons ();
		}
		ToggleSelectedUnitUI(turnOn);
	}

	public void ToggleCombatPanelButtons () {
		Debug.Log ("ToggleCombatPanelButtons");
		if (gameManager.combatManager.targetingActive) {
			if (combatPanelButtons [0].gameObject.activeSelf == false)
				combatPanelButtons [0].gameObject.SetActive(true);
			for (int i = 1; i < combatPanelButtons.Length; i++) {
				if (combatPanelButtons [i].gameObject.activeSelf == true)
					combatPanelButtons [i].gameObject.SetActive(false);
			}
		} else {
			if (combatPanelButtons [0].gameObject.activeSelf == true)
				combatPanelButtons [0].gameObject.SetActive(false);
			for (int i = 1; i < combatPanelButtons.Length; i++) {
				if (i <= storedNumCombatActions && combatPanelButtons [i].gameObject.activeSelf == false)
					combatPanelButtons [i].gameObject.SetActive(true);
				else if (i > storedNumCombatActions && combatPanelButtons [i].gameObject.activeSelf == true)
					combatPanelButtons [i].gameObject.SetActive(false);
			}
		}
	}

	public void ToggleUnitDetail(bool turnOn) {
		if (turnOn) {
			detailPanel.SetActive (true);
		} else {
			detailPanel.SetActive (false);
			//Deactivate Build Mode
		}
	}

	public void ToggleBuildUI(bool turnOn) {
		if (turnOn) {
			ToggleSelectedUnitUI (false);

			startWaveButton.gameObject.SetActive (false);

			buildPanel.SetActive(true);
			switchBuildMenusButton.gameObject.SetActive (true);
			buildNoteText.enabled = true;
		} else {
			buildPanel.SetActive(false);
			switchBuildMenusButton.gameObject.SetActive (false);
			buildNoteText.enabled = false;

			startWaveButton.gameObject.SetActive (true);
		}
	}

	public void ToggleCombatUI(bool turnOn) {
		if (turnOn) {
			ToggleSelectedUnitUI (false);

			cashText.enabled = false;
			buildToggle.gameObject.SetActive (false);
			startWaveButton.gameObject.SetActive (false);

			waveNumText.enabled = true;
			turnStatusText.enabled = true;
			nextTurnButton.gameObject.SetActive (true);

		} else {
			ToggleSelectedUnitUI (false);

			waveNumText.enabled = false;
			turnStatusText.enabled = false;
			nextTurnButton.gameObject.SetActive (false);

			cashText.enabled = true;
			buildToggle.gameObject.SetActive (true);
			startWaveButton.gameObject.SetActive (true);
		}
	}

	public void ToggleCombatPanel(bool turnOn) {
		if (turnOn) {
			combatPanel.SetActive (true);
		} else {
			combatPanel.SetActive (false);
		}
	}

	public void UpdateCashText(long cash) {
		cashText.text = "$" + cash;
	}
		
	public void UpdateNameText (string unitName) {
		selectedUnitText.text = unitName;
	}

	public void UpdateVitalsText(int currentHP, int maxHP, int currentAP, int maxAP) {
		if (maxAP != 0) {
			vitalStatsText.text = "HP " + currentHP + "/" + maxHP + "   AP " + currentAP + "/" + maxAP;
		} else {
			vitalStatsText.text = "HP " + currentHP + "/" + maxHP;
		}
	}

	public void UpdateDetailsText(string status, int maxHP, int atk, int def, int maxAP, string special) {
		detailPanel.GetComponentInChildren<Text> ().text = "Status: " + status + "\nHP: " + maxHP + "\nAtk: " + atk + "\nDef: " + def + "\nAP: " + maxAP + "\nSpecial: " + special;
	}

	public void UpdateWaveNumberText(int waveNum) {
		waveNumText.text = "Wave " + waveNum;
	}

	public void ToggleTurnText(bool isPlayerTurn) {
		if (isPlayerTurn) {
			turnStatusText.text = "Player Turn";
		} else {
			turnStatusText.text = "Enemy Turn";
		}
	}

	public void RenderPathLine(List<Vector3> pathList) {
		//NOTE: the reason why the lines are rendering weirdly on right angles is because its trying to draw corners between rectangular segments
		pathRenderLine.enabled = true;
		pathRenderLine.SetVertexCount (pathList.Count);
		Vector3[] pathArray = new Vector3[pathList.Count];
		for (int i = 0; i < pathList.Count; i++) {
			pathArray [i] = new Vector3 (pathList [i].x, pathList [i].y, pathList [i].z - 0.5f);
		}
	pathRenderLine.SetPositions (pathArray);
	}

	public void UnrenderPathLine() {
		pathRenderLine.enabled = false;
	}
	// Update is called once per frame
	//void Update () {
	//}
}
