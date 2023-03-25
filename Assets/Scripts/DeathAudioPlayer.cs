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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DeathSounds() {
        // audio.PlayOneShot(gunshotSound);
        gunshotAudio.Play();
        yield return new WaitForSeconds(1.5f);
        
        // AudioClip randomGoreSound = goreSounds[Random.Range(0, goreSounds.Length)];
        // audio.PlayOneShot(randomGoreSound);
        // audio.PlayOneShot(dropGunSound);'
        gunDropAudio.Play();
        yield return new WaitForSeconds(1.0f);

        // audio.PlayOneShot(bodyThudSound);
        bodyThudAudio.Play();

        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.DeathFinished();
    }
}
