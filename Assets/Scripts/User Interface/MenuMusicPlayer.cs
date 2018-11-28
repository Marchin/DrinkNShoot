using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource themeSong;
    [SerializeField] float musicStartDelay = 2.5f;

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