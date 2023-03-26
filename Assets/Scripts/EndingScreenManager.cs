using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class EndingScreenManager : MonoBehaviour {

    public GameObject statsPanel;
    
    // Start is called before the first frame update
    void Start() {
        GameManager.Instance.dRunner.StartDialogue("Ending");
    }

    // Update is called once per frame
    void Update() {
        
    }
    
    [YarnCommand("load_credits")]
    public void LoadCredits() {
        SceneManager.LoadScene("Credits");
    }
}
