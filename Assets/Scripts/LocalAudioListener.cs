using Fusion;
using UnityEngine;

public class LocalAudioListener : NetworkBehaviour
{
    private AudioListener audioListener;

    void Start()
    {
        audioListener = GetComponent<AudioListener>();

        if (!Object.HasInputAuthority)
        {
            audioListener.enabled = false;
        }
    }
}
