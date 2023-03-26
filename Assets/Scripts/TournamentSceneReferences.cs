using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentSceneReferences : MonoBehaviour {

    public GameObject revolver;
    public DeathAudioPlayer deathAudioPlayer;
    public GameObject bagOnHead;

    public void Start() {
        // start day is called here because we need to ensure that all our objects are loaded in
        GameManager.Instance.StartDay();
    }
    
}
