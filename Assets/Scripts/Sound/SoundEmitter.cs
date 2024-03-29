using UnityEngine;
using UnityEngine.UI;

/* SoundEmitter
 * 
 * Author: Mikael Sollenborn
 * 
 * Purpose: Play a sound effect as a condition becomes true, possibly looping the sound
 * 
 * Dependencies: SoundManagerInitializer.cs
 * 				 SoundManager.cs
 * 
 * Usage:
 *		 Currently, thesound can be started from elsewhere through code (remote), start right away, hook into a button, start when a collision with the object happens, or when rigidbodies reach a certain speed (slightly random leftover from Curling Buddies)
 */

public class SoundEmitter : MonoBehaviour {

	public enum EmitterType { RemoteControlled, Immediate, OnButtonPress, OnCollideAll, OnCollideTag, OnCollideOtherThanTag, OnCollapse };

	public EmitterType emitterType = EmitterType.OnCollideAll;

	public SingleSfx [] singleSfx;
	public SfxRandomType randomSfx = SfxRandomType.None;

	public float soundPlayProbability = 1;

	public string collideTag = string.Empty;
	public bool collideTagInParent = false;

	public float volume = 1;
	public float pitch = -1;
	public bool randomPitch = false;
	public float startDelay = 0;
	public Vector2 immediateDelayRandomTime = Vector2.zero;

	public int repeatSoundNof = 0; // <0 = infinite
	private int repeatCounter = 0;
	public float repeatDelayExtraTime = 0;
	public Vector2 repeatDelayExtraRandomTime = Vector2.zero;
	public bool repeatSameSound = true;
	private int lastPlayedIndex = -1, repeatIndex = -1;
	public bool stopOnHitObjectStopTouch = false;
	private GameObject lastHitObject = null;
	public bool fadeStop = true;

	public float minTriggerDelay = 0.25f;
	private float minTriggerTimer = 0;

	public Rigidbody[] fallWatch;
	public Vector3 fallSpeedTresholds = Vector3.zero;
	public bool singleFall = true;
	private bool hasFallen = false;

	private bool stillHitting = false;
	private int frameCount = 0;

	public Button altTriggerButton = null;


	void Start () {
		if (emitterType == EmitterType.Immediate) {
			repeatCounter = repeatSoundNof;
			Invoke ("PlaySoundLoop", Random.Range (immediateDelayRandomTime.x, immediateDelayRandomTime.y));
		}

		if (emitterType == EmitterType.OnButtonPress)
		{
			Button b = altTriggerButton;
			if (!b)
				b = GetComponentInChildren<Button>();
			if (b)
				b.onClick.AddListener(PlaySound);
		}
	}

	void Update () {
		if (minTriggerTimer > 0)
			minTriggerTimer -= Time.deltaTime;

		if (emitterType == EmitterType.OnCollapse && fallWatch != null && fallWatch.Length > 0 && fallSpeedTresholds != Vector3.zero && minTriggerTimer <= 0) {

			if (singleFall && hasFallen)
				return;

			int nofFalling = 0;
			foreach (Rigidbody rb in fallWatch) {
				if (fallSpeedTresholds.x != 0 && ((fallSpeedTresholds.x < 0 && rb.velocity.x < fallSpeedTresholds.x) || (fallSpeedTresholds.x > 0 && rb.velocity.x > fallSpeedTresholds.x)))
					nofFalling++;
				else if (fallSpeedTresholds.y != 0 && ((fallSpeedTresholds.y < 0 && rb.velocity.y < fallSpeedTresholds.y) || (fallSpeedTresholds.y > 0 && rb.velocity.y > fallSpeedTresholds.y)))
					nofFalling++;
				else if (fallSpeedTresholds.z != 0 && ((fallSpeedTresholds.z < 0 && rb.velocity.z < fallSpeedTresholds.z) || (fallSpeedTresholds.z > 0 && rb.velocity.z > fallSpeedTresholds.z)))
					nofFalling++;
				// print (rb.velocity);
			}

			if (nofFalling == fallWatch.Length) {
				hasFallen = true;
				minTriggerTimer = minTriggerDelay;
				PlaySound ();
			}
		}
	}
		
	void OnTriggerEnter(Collider collider)
	{
		if (emitterType == EmitterType.RemoteControlled || emitterType == EmitterType.Immediate || emitterType == EmitterType.OnCollapse)
			return;

		if (lastHitObject != null && stopOnHitObjectStopTouch)
			return;

		if (minTriggerTimer > 0)
			return;

		bool doPlay = true;
		GameObject findMe = null;

		if (emitterType != EmitterType.OnCollideAll) {

			findMe = collider.gameObject;
			if (collideTag != string.Empty)
			{
				if (!collideTagInParent && collider.gameObject.tag != collideTag)
					findMe = null;
				if (collideTagInParent)
					findMe = GameUtil.FindParentWithTag(collider.gameObject, collideTag);
			}

			if (findMe)
			{
				if (emitterType == EmitterType.OnCollideOtherThanTag)
					doPlay = false;
			}
		}

		if (doPlay) {
			lastHitObject = findMe != null ?findMe : collider.gameObject;
			stillHitting = true;

			PlaySound ();
			minTriggerTimer = minTriggerDelay;
		}
	}

	void OnCollisionEnter(Collision collision) {
		OnTriggerEnter (collision.collider);
	}

	void OnTriggerStay(Collider collider)
	{
		if (!stopOnHitObjectStopTouch || lastHitObject == null)
			return;

		GameObject match = null;

		if (emitterType != EmitterType.OnCollideAll)
		{
			if (collideTag != string.Empty)
			{
				if (!collideTagInParent && collider.gameObject.tag == collideTag)
					match = collider.gameObject;
				if (collideTagInParent)
					match = GameUtil.FindParentWithTag(collider.gameObject, collideTag);
			}

			if (match == null)
			{
				if (emitterType != EmitterType.OnCollideOtherThanTag)
					match = collider.gameObject;
			}
		} else
			match = collider.gameObject;

		if (lastHitObject == match) {
			stillHitting = true;
		}
	}


	void OnCollisionStay(Collision collision) {
		OnTriggerStay (collision.collider);
	}


	public void PlaySound() {
		if (!this.enabled)
			return;

		if (soundPlayProbability < 1 && Random.Range(0f, 1f) > soundPlayProbability)
			return;

		repeatCounter = repeatSoundNof;
		PlaySoundLoop ();
	}

	private void PlaySoundLoop() {
		float playTime = 0;

		if (randomSfx != SfxRandomType.None) {
			if (repeatIndex != -1 && repeatSameSound)
				playTime = SoundManager.GetInstance().PlayRandomFromType (randomSfx, repeatIndex, startDelay, volume, pitch, randomPitch);
			else {
				playTime = SoundManager.GetInstance().PlayRandomFromType (randomSfx, -1, startDelay, volume, pitch, randomPitch);
				repeatIndex = SoundManager.GetInstance().lastRandomIndex;
			}
		}
		else if (singleSfx != null && singleSfx.Length > 0) {
			lastPlayedIndex = Random.Range (0, singleSfx.Length);
			if (repeatIndex != -1 && repeatSameSound)
				lastPlayedIndex = repeatIndex;
			else
				repeatIndex = lastPlayedIndex;
			playTime = SoundManager.GetInstance().PlaySingleSfx (singleSfx [lastPlayedIndex], randomPitch, startDelay, volume, pitch);
		}

		if (repeatCounter != 0 && playTime > 0) {
			repeatCounter--;
			Invoke ("PlaySoundLoop", playTime + repeatDelayExtraTime + Random.Range(repeatDelayExtraRandomTime.x, repeatDelayExtraRandomTime.y));
		} else
			repeatIndex = -1;
	}

	public void StopPlay(bool stopForced = true) {
		repeatCounter = 0;
		repeatIndex = -1;
		CancelInvoke ("PlaySoundLoop");
		if (stopForced) {
			if (randomSfx == SfxRandomType.None) {
				if (lastPlayedIndex >= 0) {
					if (fadeStop)
						SoundManager.GetInstance().FadeSingleSfx (singleSfx [lastPlayedIndex]);
					else
						SoundManager.GetInstance().StopSingleSfx (singleSfx [lastPlayedIndex]);
				}
			} else {
				if (fadeStop)
					SoundManager.GetInstance().FadeRandomPlayingSfx (randomSfx);
				else
					SoundManager.GetInstance().StopPlayingRandomSfx (randomSfx);
			}
		}
		lastPlayedIndex = -1;
	}

	public void SetSingleSfx(SingleSfx sfx) {
		singleSfx = new SingleSfx[] { sfx };
	}

	void LateUpdate()
	{
		if (stillHitting == false && lastHitObject != null && stopOnHitObjectStopTouch) {
			frameCount++;
			if (frameCount > 20) {
				lastHitObject = null;
				StopPlay ();
				frameCount = 0;
			}
		} else
			frameCount = 0;
		stillHitting = false;
	}

}
