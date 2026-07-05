using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Range(0f, 1f)]
    public float volume = 1f;

    void Start()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }

    public void SetVolume(float v)
    {
        volume = v;

        if (audioSource != null)
        {
            audioSource.volume = v;
        }
    }
}