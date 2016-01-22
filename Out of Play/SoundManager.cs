using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource sfxSource;
	public AudioSource musicSource;
	public float lowPitchRange = 0.95f;
	public float highPitchRange = 1.05f;
	public AudioClip laserSFX;
	public AudioClip explosionSFX;
	public AudioClip psySFX;
	public AudioClip electricSFX;
	public AudioClip[] slashSFXArray;
	public AudioClip footstepsSFX;
	//public AudioClip robotFootstepsSFX;
	public AudioClip breakWallSFX;
	public AudioClip buildMachineSFX;
	public AudioClip deviceActivateSFX;
	public AudioClip deviceDeactivateSFX;
	public AudioClip[] captainTargetSFX;
	public AudioClip hasteSFX;
	public AudioClip whooshSFX;
	public AudioClip buildMusic;
	public AudioClip lowCombatMusic;
	public AudioClip highCombatMuisc;
	public bool raisedTension;
	private bool loopSound;

	void Start() {
		loopSound = false;
		raisedTension = false;
	}

	private void PlaySingle(AudioClip clip, bool useRandomPitch) {
		sfxSource.Stop ();
		if (useRandomPitch) {
			float randompitch = Random.Range (lowPitchRange, highPitchRange);
			sfxSource.pitch = randompitch;
		} else {
			sfxSource.pitch = 1f;
		}
		sfxSource.clip = clip;
		sfxSource.Play ();
	}

	private void PlaySingle(AudioClip[] clipArray, bool useRandomPitch) {
		AudioClip clip = clipArray [Random.Range (0, clipArray.Length)];
		PlaySingle (clip, useRandomPitch);
	}

	private IEnumerator PlayLoop(AudioClip clip, bool useRandomPitch) {
		if (useRandomPitch) {
			float randompitch = Random.Range (lowPitchRange, highPitchRange);
			sfxSource.pitch = randompitch;
		} else {
			sfxSource.pitch = 1f;
		}
		sfxSource.loop = true;
		sfxSource.clip = clip;
		sfxSource.Play ();
		yield return new WaitWhile (() => loopSound);
		sfxSource.loop = false;
	}

	public void MuteAudio() {
		if (musicSource.mute) {
			sfxSource.mute = false;
			musicSource.mute = false;
		} else {
			sfxSource.mute = true;
			musicSource.mute = true;
		}
	}

	public void CheckRaisedTension() {
		if (raisedTension && musicSource.clip != highCombatMuisc) {
			musicSource.clip = highCombatMuisc;
			musicSource.Play ();
		}
	}

	public void SetCombatMusic() {
		musicSource.clip = lowCombatMusic;
		musicSource.Play ();
	}

	public void SetNonCombatMusic() {
		musicSource.clip = buildMusic;
		musicSource.Play ();
	}

	public void PlayLaserSFX() {
		PlaySingle (laserSFX, true);
	}

	public void PlayExplosionSFX() {
		PlaySingle (explosionSFX, true);
	}

	public void PlaySlashSFX() {
		PlaySingle (slashSFXArray, true);
	}

	public void PlayBreakWallSFX() {
		PlaySingle (breakWallSFX, true);
	}

	public void PlayBuildMachineSFX() {
		PlaySingle (buildMachineSFX, true);
	}

	public void playElectricSFX() {
		PlaySingle (electricSFX, true);
	}

	public void PlayPsySFX() {
		loopSound = true;
		StartCoroutine(PlayLoop (psySFX, false));
	}

	public void PlayActivateSFX() {
		PlaySingle (deviceActivateSFX, true);
	}

	public void PlayDeactivateSFX() {
		PlaySingle (deviceDeactivateSFX, true);
	}

	public void PlayHasteSFX() {
		PlaySingle (hasteSFX, true);
	}

	public void PlayWhooshSFX() {
		PlaySingle (whooshSFX, true);
	}

	public void PlayTargetVO() {
		PlaySingle (captainTargetSFX, false);
	}

	public void PlayWalkSFX() {
		loopSound = true;
		StartCoroutine(PlayLoop (footstepsSFX, false));
	}

	public void StopSFXLoop() {
		loopSound = false;
		sfxSource.Stop ();
		//StartCoroutine (StopSoundGracefully ());
	}

	//TODO: this technically would work, but if you click quickly enough it gets stuck with no volume
//	public IEnumerator StopSoundGracefully() {
//		float storedVolume = sfxSource.volume;
//		for (int i = 0; i < 15; i++) {
//			sfxSource.volume /= 1.3f;
//			yield return new WaitForFixedUpdate();
//		}
//		sfxSource.Stop ();
//		sfxSource.volume = storedVolume;
//	}
}
