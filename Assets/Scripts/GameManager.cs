using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public AudioSource audio;

    public GunManager gunManager;
    public DeathAudioPlayer deathAudioPlayer;
    public static GameManager Instance { get; private set; }
    private void Awake() { 
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }
    
    
    
    // Start is called before the first frame update
    void Start() {
        gunManager.TakeOutGun();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinishSpin() {
        gunManager.PutAwayGun();
        deathAudioPlayer.StartCoroutine("DeathSounds");
    }

    public void LoadOtherWorld() {
        SceneManager.LoadScene("OtherWorldScene");
    }
}
