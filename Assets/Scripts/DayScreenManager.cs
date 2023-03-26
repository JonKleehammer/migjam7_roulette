using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class DayScreenManager : MonoBehaviour {

    public float duration = 5f;
    private int dayNum;
    
    public TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start() {
        dayNum = GameManager.Instance.dayNum;

        if (dayNum <= 5)
            text.text = $"Day {dayNum}";
        else
            text.text = "Freedom.";
    }

    // Update is called once per frame
    void Update() {
        duration -= Time.deltaTime;
        if (duration > 0f)
            return;
        
        if (dayNum <= 5)
            SceneManager.LoadScene("TournamentScene");
        else {
            SceneManager.LoadScene("EndingScene");
            GameManager.Instance.dRunner.StartDialogue("Ending");
        }
    }
}
