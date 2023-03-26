using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class DayScreenManager : MonoBehaviour {

    public float duration = 3f;
    
    public TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(NextScene());
    }


    IEnumerator NextScene() {
        var dayNum = GameManager.Instance.dayNum;
        if (dayNum <= 5)
            text.text = $"Day {dayNum}";
        else
            text.text = "Freedom.";

        while (duration > 0f) {
            duration -= Time.deltaTime;
            yield return null;
        }
        
        if (dayNum <= 5)
            SceneManager.LoadScene("TournamentScene");
        else
            SceneManager.LoadScene("EndingScene");
    }
}
