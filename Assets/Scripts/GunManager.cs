using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour {

    private AudioSource audio;
    public AudioClip takeOutSound;
    public AudioClip putAwaySound;
    
    // Start is called before the first frame update
    void Start() {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TakeOutGun() {
        gameObject.SetActive(true);
        audio.PlayOneShot(takeOutSound);
    }

    public void PutAwayGun() {
        gameObject.SetActive(false);
        audio.PlayOneShot(putAwaySound);
    }
}
