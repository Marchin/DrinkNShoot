using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Crow))]
[RequireComponent(typeof(Life))]
public class CrowAudio : MonoBehaviour
{
	[SerializeField] AudioSource[] crowSounds;
	[SerializeField] AudioSource attackSound;
	[SerializeField] AudioSource deathSound;
	[SerializeField] [Range(0f, 100f)]	
	float makeSoundProbability = 25f;
	
	static bool crowMakingSound = false;
	
	Crow crow;
	Life crowLife;

	void Awake()
	{
		crow = GetComponentInParent<Crow>();
		crowLife = GetComponentInParent<Life>();
	}

	void Start()
	{
		crow.OnAttack.AddListener(PlayAttackSound);
		crowLife.OnDeath.AddListener(PlayDeathSound);

		crowLife.DeathLength = deathSound.clip.length;
	}

	void Update()
	{
		PlayRandomCrowSound();
	}

	void PlayRandomCrowSound()
	{
		if (!crowMakingSound && Random.Range(0f, 100f) < makeSoundProbability)
		{
			crowMakingSound = true;
			int soundIndex = Random.Range(0, crowSounds.Length);
			crowSounds[soundIndex].Play();
			Invoke("ReEnableSoundPlayback", crowSounds[soundIndex].clip.length);
		}
	}

	void PlayAttackSound()
	{
		attackSound.Play();
	}

	void PlayDeathSound()
	{	
		deathSound.Play();
	}

	void ReEnableSoundPlayback()
	{
		crowMakingSound = false;
	}
}