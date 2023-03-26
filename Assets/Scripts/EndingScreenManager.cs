using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class EndingScreenManager : MonoBehaviour {

    public GameObject statsPanel;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
    
    [YarnCommand("load_stats")]
    public void OpenStatsPanel() {
        statsPanel.SetActive(true);
    }
}
