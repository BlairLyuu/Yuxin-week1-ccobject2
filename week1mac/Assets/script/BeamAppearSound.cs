using UnityEngine;

public class BeamAppearSound : MonoBehaviour
{
    public AudioClip appearSound;

    void OnEnable()
    {
        // beam被激活时自动播放
        if (appearSound != null)
        {
            AudioSource.PlayClipAtPoint(appearSound, transform.position);
        }
    }
}