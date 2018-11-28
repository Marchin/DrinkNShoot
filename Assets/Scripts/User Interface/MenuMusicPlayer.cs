using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource themeSong;
    [SerializeField] float musicStartDelay = 2f;

	float musicTimer = 0f;

    void Update()
    {
        if (!themeSong.isPlaying)
        {
            musicTimer += Time.deltaTime;
            if (musicTimer >= musicStartDelay)
            {
                musicTimer = 0f;
                themeSong.Play();
            }
        }
    }
}