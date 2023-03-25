using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayScreenManager : MonoBehaviour {

    public float duration = 5f;

    public TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start() {
        text.text = $"Day {GameManager.Instance.dayNum}";
    }

    // Update is called once per frame
    void Update() {
        duration -= Time.deltaTime;
        if (duration > 0f)
            return;
        
        SceneManager.LoadScene("TournamentScene");
        GameManager.Instance.StartDay();
    }
}
