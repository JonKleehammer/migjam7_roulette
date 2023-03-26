using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager : MonoBehaviour {

    public DialogueRunner dRunner;
    public AudioSource audio;

    // public TournamentSceneReferences sceneReferences;
    
    private DeathAudioPlayer deathAudioPlayer;

    private GameObject revolver;
    public AudioClip takeOutSound;
    public AudioClip putAwaySound;
    public CylinderSpinner spinner;

    public int dayNum = 1;
    private int deathNum;

    public int maxRounds = 4;
    private int currentRound;
    private int deathRound;
    private bool playerWillDie;
    public bool playerTurn = true;
    private bool wasGoodSpin;

    private GameObject bagOnHead;
    public AudioClip removeBagSound;

    public AudioClip gunCockSound;
    public AudioClip dryFireSound;
    public AudioClip gunshotSound;

    private GameObject gunToHeadButton;
    private GameObject pullTriggerButton;
    private GameObject handOffGunButton;

    public static GameManager Instance { get; private set; }
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    
    
    
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsDialogueRunning() {
        return dRunner.IsDialogueRunning;
    }

    public void StartDay() {
        print("Start Day");
        GetReferences();
        currentRound = 0;
        playerTurn = true;
        for (int i = 0; i < 10; i++) {
            AssignDeath();
            print($"player: {playerWillDie} round: {deathRound}");
        }
        AssignDeath();
        dRunner.StartDialogue($"Day{dayNum}Start");
    }

    private void GetReferences() {
        var references = FindObjectOfType<TournamentSceneReferences>();
        revolver = references.revolver;
        spinner = references.spinner;
        bagOnHead = references.bagOnHead;
        deathAudioPlayer = references.deathAudioPlayer;
        gunToHeadButton = references.gunToHeadButton;
        pullTriggerButton = references.pullTriggerButton;
        handOffGunButton = references.handOffGunButton;
    }
    
    public void AssignDeath() {
        // first 2 days are scripted
        switch (dayNum) {
            case 1:
                deathRound = 3;
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
        playerWillDie = Random.Range(0, 2) == 0;
    }

    [YarnCommand("remove_bag")]
    public void RemoveBag() {
        bagOnHead.SetActive(false);
        audio.PlayOneShot(removeBagSound);
    }

    [YarnCommand("start_new_round")]
    public void StartNewRound() {
        print("Start new round");
        playerTurn = true;
        spinner.waitingForSpin = true;
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

    public void FinishSpin(bool goodSpin) {
        wasGoodSpin = goodSpin; // used frequently in other parts of the code so storing it
        if (!wasGoodSpin) {
            dRunner.StartDialogue("WeakSpin");
            spinner.waitingForSpin = true;
            return;
        }
        
        print($"Spin finsihed for round {currentRound}. player:{playerTurn}");
        if (playerTurn)
            PlayerFinishSpin();
        else
            StartCoroutine(FinishOpponentSpin());
    }

    public void PlayerFinishSpin() {
        // activate button to put gun to head
        gunToHeadButton.SetActive(true);
    }

    public void PutGunToHead() {
        //active gun to pull trigger
        revolver.SetActive(false);
        audio.PlayOneShot(gunCockSound);
        gunToHeadButton.SetActive(false);
        pullTriggerButton.SetActive(true);
    }

    public void PullTrigger() {
        pullTriggerButton.SetActive(false);
        if (playerWillDie && deathRound <= currentRound)
            KillPlayer();
        else {
            audio.PlayOneShot(dryFireSound);
            handOffGunButton.SetActive(true);
        }
    }

    public void HandOffGun() {
        handOffGunButton.SetActive(false);
        playerTurn = false;
        PutAwayGun();
        StartCoroutine(SimulateOpponentTurn());
    }

    IEnumerator SimulateOpponentTurn() {
        print("starting opponent turn");
        yield return new WaitForSeconds(1f);
        print("opponent taking out gun");
        revolver.transform.position += Vector3.up * 10f;
        TakeOutGun();
        yield return new WaitForSeconds(1f);
        print("opponent spinning");
        spinner.Spin();
    }
    

    IEnumerator FinishOpponentSpin() {
        print("opponent finished spinning");
        yield return new WaitForSeconds(1f);
        print("opponent cocking gun");
        audio.PlayOneShot(gunCockSound);
        yield return new WaitForSeconds(1f);
        print("opponent pulling trigger");

        // if they die
        if (!playerWillDie && deathRound <= currentRound) {
            print("opponent dies");
            KillOpponent();
            yield break;
        }
        
        print("opponent dry fires");
        audio.PlayOneShot(dryFireSound);
        yield return new WaitForSeconds(1f);
        PutAwayGun();
        yield return new WaitForSeconds(1f);
        print("opponent return gun");
        revolver.transform.position += Vector3.down * 10f;
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
        bagOnHead.SetActive(true);
        revolver.SetActive(false);
        deathNum += 1;
        deathAudioPlayer.StartCoroutine("DeathSounds");
    }

    public void KillOpponent() {
        deathAudioPlayer.StartCoroutine("DeathSounds");
    }

    public void LoadOtherWorld() {
        dayNum += 1;
        SceneManager.LoadScene("OtherWorldScene");
        dRunner.StartDialogue($"Death{deathNum}Start");
    }
    
    [YarnCommand("load_next_day")]
    public void LoadNextDay() {
        SceneManager.LoadScene("NextDayScene");
    }
    
    public void FinishDay() {
        if (dayNum < 5)
            dRunner.StartDialogue("FinishDay");
        else
            dRunner.StartDialogue("Survived");
        dayNum += 1;
    }
    
    [YarnCommand("play_gunshot")]
    public void GunShotSound() {
        audio.PlayOneShot(gunshotSound);
    }
}
