using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CrowSound : MonoBehaviour
{
	[SerializeField] AudioSource[] crowSounds;
	[SerializeField] [Range(0, 100)]
	float makeSoundProbability = 45f;
	static bool crowMakingSound = false;

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

	void NotMakingSound()
	{
		crowMakingSound = false;
	}
}
