using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentSceneReferences : MonoBehaviour {

    public GameObject revolver;
    public CylinderSpinner spinner;
    public DeathAudioPlayer deathAudioPlayer;
    public GameObject bagOnHead;
    public GameObject gunToHeadButton;
    public GameObject pullTriggerButton;
    public GameObject handOffGunButton;
    
    public void Start() {
        gunToHeadButton.GetComponent<Button>().onClick.AddListener(GameManager.Instance.PutGunToHead);
        pullTriggerButton.GetComponent<Button>().onClick.AddListener(GameManager.Instance.PullTrigger);
        handOffGunButton.GetComponent<Button>().onClick.AddListener(GameManager.Instance.HandOffGun);
        
        // start day is called here because we need to ensure that all our objects are loaded in
        GameManager.Instance.StartDay();
    }
    
}
