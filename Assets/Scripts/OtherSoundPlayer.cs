using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherSoundPlayer : MonoBehaviour {

    public AudioSource audio;
    public AudioClip[] sounds;

    private float timeBeforeSound;
    public float minTimeBeforeSound;
    public float maxTimeBeforeSound;
    
    // Start is called before the first frame update
    void Start() {
        timeBeforeSound = maxTimeBeforeSound;
    }

    // Update is called once per frame
    void Update() {
        timeBeforeSound -= Time.deltaTime;
        if (timeBeforeSound > 0f)
            return;
        timeBeforeSound = Random.Range(minTimeBeforeSound, maxTimeBeforeSound);

        AudioClip soundToPlay = sounds[Random.Range(0, sounds.Length)];
        audio.PlayOneShot(soundToPlay);
    }
}
