using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAudioPlayer : MonoBehaviour {

    public AudioSource audio;
    public AudioClip gunshotSound;
    public AudioClip[] goreSounds;
    public AudioClip dropGunSound;
    public AudioClip bodyThudSound;

    public AudioSource gunshotAudio;
    public AudioSource gunDropAudio;
    public AudioSource bodyThudAudio;

    IEnumerator DeathSounds() {
        gunshotAudio.Play();
        yield return new WaitForSeconds(1.5f);
        
        gunDropAudio.Play();
        yield return new WaitForSeconds(1.0f);

        bodyThudAudio.Play();
        yield return new WaitForSeconds(1.5f);
        
        GameManager.Instance.DeathFinished();
    }
}
