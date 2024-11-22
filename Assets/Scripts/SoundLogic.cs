using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLogic : MonoBehaviour
{
    GameLogic game;
    private AudioSource audioPlayer;
    [SerializeField] public AudioClip carDoorOpenClose;
    public void PlaySound(AudioClip audioClip)
    {
        audioPlayer.PlayOneShot(audioClip);
    }
    public void PlaySound(AudioClip audioClip, Vector3 location)
    {
        AudioSource.PlayClipAtPoint(audioClip, location);
    }
    // Start is called before the first frame update
    void Start()
    {
        game = GameLogic.instance;
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        audioPlayer.volume = game.settings.volume / 100f;
    }
}
