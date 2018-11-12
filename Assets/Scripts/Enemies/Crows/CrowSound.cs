using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CrowSound : MonoBehaviour
{
	[SerializeField] AudioSource[] crowSounds;
	[SerializeField] AudioSource attackSound;

	[SerializeField] [Range(0f, 100f)]
	float makeSoundProbability = 45f;
	
	static bool crowMakingSound = false;
	
	Crow crow;

	void Awake()
	{
		crow = GetComponentInParent<Crow>();
	}

	void Start()
	{
		crow.OnAttack.AddListener(PlayAttackSound);
	}

	void PlayRandomCrowSound()
	{
		if (!crowMakingSound && Random.Range(0f, 100f) < makeSoundProbability)
		{
			crowMakingSound = true;
			int soundIndex = Random.Range(0, crowSounds.Length);
			crowSounds[soundIndex].Play();
			Invoke("NotMakingSound", crowSounds[soundIndex].clip.length);
		}
	}

	void PlayAttackSound()
	{
		attackSound.Play();
	}

	void NotMakingSound()
	{
		crowMakingSound = false;
	}
}