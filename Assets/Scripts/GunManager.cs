using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour {

    public AudioClip takeOutSound;
    public AudioClip putAwaySound;
    
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TakeOutGun() {
        gameObject.SetActive(true);
        GameManager.Instance.audio.PlayOneShot(takeOutSound);
    }

    public void PutAwayGun() {
        gameObject.SetActive(false);
        GameManager.Instance.audio.PlayOneShot(putAwaySound);
    }
}
