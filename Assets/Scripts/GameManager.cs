using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager : MonoBehaviour {

    public DialogueRunner dRunner;
    public AudioSource audio;

    public DeathAudioPlayer deathAudioPlayer;

    public GameObject revolver;
    public AudioClip takeOutSound;
    public AudioClip putAwaySound;
    public CylinderSpinner spinner;

    private int dayNum;
    private int deathNum;

    public int maxRounds = 4;
    private int currentRound;
    private int deathRound;
    private bool playerWillDie;
    private bool playerTurn = true;

    public static GameManager Instance { get; private set; }
    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    
    
    
    // Start is called before the first frame update
    void Start() {
        StartDay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsDialogueRunning() {
        return dRunner.IsDialogueRunning;
    }

    public void StartDay() {
        dayNum += 1;
        AssignDeath();
        dRunner.StartDialogue($"Day{dayNum}Start");
    }
    
    public void AssignDeath() {
        // first 2 days are scripted
        switch (dayNum) {
            case 1:
                deathRound = 1;
                playerWillDie = false;
                return;
            case 2:
                deathRound = 2;
                playerWillDie = true;
                return;
        }
        
        // the rest are based on luck, but with an assigned turn of death and who will die
        // this is to ensure we have enough dialogue and that someone will die in a reasonable time frame
        deathRound = Random.Range(1, maxRounds + 1);
    }

    [YarnCommand("start_new_round")]
    public void StartNewRound() {
        currentRound += 1;
        TakeOutGun();
    }
    
    public void TakeOutGun() {
        revolver.SetActive(true);
        audio.PlayOneShot(takeOutSound);
    }

    public void PutAwayGun() {
        revolver.SetActive(false);
        audio.PlayOneShot(putAwaySound);
    }

    public void FinishSpin(bool wasGoodSpin) {
        print($"Spin finsihed for round {currentRound}. player:{playerTurn}");
        if (playerTurn)
            PlayerFinishSpin(wasGoodSpin);
        else
            OpponentFinishSpin();
    }

    public void PlayerFinishSpin(bool wasGoodSpin) {
        if (!wasGoodSpin) {
            dRunner.StartDialogue("WeakSpin");
            return;
        }

        if (playerWillDie && deathRound == currentRound)
            KillPlayer();
        else
            StartCoroutine(SimulateOpponentTurn());
    }

    IEnumerator SimulateOpponentTurn() {
        playerTurn = false;
        PutAwayGun();
        yield return new WaitForSeconds(1f);
        revolver.transform.position += Vector3.up * 10f;
        TakeOutGun();
        yield return new WaitForSeconds(1f);
        spinner.Spin();
    }

    public void OpponentFinishSpin() {
        if (!playerWillDie && deathRound == currentRound) {
            KillOpponent();
            return;
        }
        
        PutAwayGun();
        revolver.transform.position += Vector3.down * 10f;
        playerTurn = true;
        ReturnGun();
    }

    private void ReturnGun() {
        dRunner.StartDialogue("ReturnGun");
    }

    public void DeathFinished() {
        if (playerTurn)
            LoadOtherWorld();
        else
            FinishDay();
    }

    public void KillPlayer() {
        deathNum += 1;
        deathAudioPlayer.StartCoroutine("DeathSounds");
    }

    public void KillOpponent() {
        deathAudioPlayer.StartCoroutine("DeathSounds");
    }

    public void LoadOtherWorld() {
        SceneManager.LoadScene("OtherWorldScene");
        dRunner.StartDialogue($"Death{deathNum}Other");
    }
    
    [YarnCommand("load_next_day")]
    public void LoadNextDay() {
        SceneManager.LoadScene("TournamentScene");
    }
    
    public void FinishDay() {
        dRunner.StartDialogue("FinishDay");
    }
}
