using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager : MonoBehaviour {

    public DialogueRunner dRunner;
    public AudioSource audio;

    public int dayNum = 1;
    private int deathNum;

    public int maxRounds = 4;
    private int currentRound;
    private int deathRound;
    private bool playerWillDie;
    public bool playerTurn = true;
    private bool wasGoodSpin;

    private GameObject bagOnHead;
    private GameObject revolver;
    public CylinderSpinner spinner;
    private GameObject buttonTutorial;
    private GameObject gunToHeadButton;
    private GameObject pullTriggerButton;
    private GameObject handOffGunButton;
    private DeathAudioPlayer deathAudioPlayer;

    public AudioClip removeBagSound;
    public AudioClip takeOutSound;
    public AudioClip putAwaySound;
    public AudioClip gunCockSound;
    public AudioClip dryFireSound;
    public AudioClip gunshotSound;
    
    public static GameManager Instance { get; private set; }
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public bool IsDialogueRunning() {
        return dRunner.IsDialogueRunning;
    }

    public void StartDay() {
        GetReferences();
        currentRound = 0;
        playerTurn = true;
        AssignDeath();
        dRunner.StartDialogue($"Day{dayNum}Start");
    }

    private void GetReferences() {
        var references = FindObjectOfType<TournamentSceneReferences>();
        revolver = references.revolver;
        spinner = references.spinner;
        bagOnHead = references.bagOnHead;
        deathAudioPlayer = references.deathAudioPlayer;
        buttonTutorial = references.buttonTutorial;
        gunToHeadButton = references.gunToHeadButton;
        pullTriggerButton = references.pullTriggerButton;
        handOffGunButton = references.handOffGunButton;
    }
    
    public void AssignDeath() {
        // first 2 days are scripted
        switch (dayNum) {
            case 1:
                deathRound = 2;
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
        
        if (playerTurn)
            PlayerFinishSpin();
        else
            StartCoroutine(FinishOpponentSpin());
    }

    public void PlayerFinishSpin() {
        if (dayNum == 1)
            buttonTutorial.SetActive(true);
        gunToHeadButton.SetActive(true);
    }

    public void PutGunToHead() {
        if (dayNum == 1)
            buttonTutorial.SetActive(false);
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
        yield return new WaitForSeconds(1f);
        revolver.transform.position += Vector3.up * 10f;
        TakeOutGun();
        yield return new WaitForSeconds(1f);
        spinner.Spin();
    }
    

    IEnumerator FinishOpponentSpin() {
        yield return new WaitForSeconds(1f);
        audio.PlayOneShot(gunCockSound);
        yield return new WaitForSeconds(1f);

        // if they die
        if (!playerWillDie && deathRound <= currentRound) {
            KillOpponent();
            yield break;
        }
        
        audio.PlayOneShot(dryFireSound);
        yield return new WaitForSeconds(1f);
        PutAwayGun();
        yield return new WaitForSeconds(1f);
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
