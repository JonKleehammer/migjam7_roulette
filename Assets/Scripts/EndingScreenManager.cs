using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class EndingScreenManager : MonoBehaviour {
    
    void Start() {
        GameManager.Instance.dRunner.StartDialogue("Ending");
    }

    [YarnCommand("load_credits")]
    public void LoadCredits() {
        SceneManager.LoadScene("Credits");
    }
}
