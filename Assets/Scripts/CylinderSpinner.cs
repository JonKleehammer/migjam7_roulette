using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CylinderSpinner : MonoBehaviour {

    public float weakSpinThreshold;
    
    public bool waitingForSpin;
    private float timeWaitingForSpin;
    
    private bool isDragging = false;

    public Transform carveOutGroupT;
    [SerializeField]
    private Transform[] carveOuts;
    public float carveOutRange = 3f;
    public float carveOutOffset;

    private float startRotation = 0;
    private float currentRotation = 0;
    private Vector3 startMousePos;
    private Vector3 lastMousePos;
    private float spinSpeed = 0;
    private float spinDir = 1;
    private float peakSpin = 0;
    public float spinDuration;
    private float spinTime;
    public float minSpinSpeed = 0.01f;
    public float spinSpeedSoundThreshold;
    
    public GameObject spinTutorial;
    public float timeUntilTutorial = 5f;
    private float timeWithoutSpin;
    
    public AudioSource spinAudio;
    public AudioSource clickAudio;
    public AudioSource finalClick;
    public AudioClip clickSound;
    
    // Start is called before the first frame update
    void Start() {
        carveOuts = carveOutGroupT.GetComponentsInChildren<Transform>();
        carveOuts = carveOuts.Skip(1).ToArray();
    }

    // Update is called once per frame
    void Update() {
        // don't do anything if dialogue is on the screen.
        if (GameManager.Instance.IsDialogueRunning())
            return;

        if (waitingForSpin)
            timeWaitingForSpin += Time.deltaTime;
        else
            timeWaitingForSpin = 0;

        // handling player input
        if (!GameManager.Instance.playerTurn || !waitingForSpin) {
            // no interaction from the player while it's not their turn
            // or if they already spun
        }
        else if (Input.GetMouseButtonDown(0)) {
            isDragging = true;
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spinSpeed = 0;
            spinAudio.Stop();
            timeWithoutSpin = 0f;
        }
        else if (Input.GetMouseButton(0)) {
            lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var yMovement = lastMousePos.y - startMousePos.y;
            currentRotation = startRotation + yMovement;
        }
        else if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
            spinTime = 0f;
            spinSpeed = lastMousePos.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            if (spinSpeed < 0) 
                spinDir = 1;
            else 
                spinDir = -1;
            spinSpeed = Mathf.Abs(spinSpeed);
            peakSpin = spinSpeed;
        }
        
        // seeing if we should show the tutorial
        if ((!Input.GetMouseButton(0) && spinSpeed <= 0 && waitingForSpin) && ((GameManager.Instance.dayNum == 1 && GameManager.Instance.playerTurn) || (timeWaitingForSpin >= timeUntilTutorial)))
            spinTutorial.SetActive(true);
        else {
            spinTutorial.SetActive(false);
            timeWithoutSpin += Time.deltaTime;
        }
        
        // Handling the spin, slowdown, and detecting when the spin is done
        if (spinSpeed > 0) {
            currentRotation += spinSpeed * spinDir;
            spinTime += Time.deltaTime;
            spinSpeed = Mathf.Lerp(spinSpeed, 0, spinTime / spinDuration);
            spinAudio.volume = Mathf.Clamp(spinSpeed, 0, 0.5f);
            if (spinSpeed < minSpinSpeed) {
                // SPIN DONE. LOGIC TO HANDLE NEXT STEPS SHOULD GO HERE
                spinSpeed = 0f;
                spinTime = 0f;
                timeWithoutSpin = 0f;
                waitingForSpin = false;
                spinAudio.Stop();
                finalClick.PlayOneShot(clickSound);
                bool wasGoodSpin = peakSpin > weakSpinThreshold;
                GameManager.Instance.FinishSpin(wasGoodSpin);
            }
        }
        
        // handling the animation of the cylinder carveouts and the sound of the cylinder spinning
        for (int i = 0; i < carveOuts.Length; i++) {
            var carveOut = carveOuts[i];
            var newY = (currentRotation + carveOutRange * ((float) i / carveOuts.Length )) % carveOutRange;
            if (newY < 0)
                newY += carveOutRange;
            newY += carveOutOffset;
            
            if (!isDragging && spinSpeed > spinSpeedSoundThreshold && !spinAudio.isPlaying)
                spinAudio.Play();
            else if (spinSpeed <= spinSpeedSoundThreshold 
                     && (spinSpeed > minSpinSpeed + 0.01f || isDragging) // added in a minimum spin speed to leave a small silent gap before we play the final click sound 
                     && Mathf.Abs(carveOut.localPosition.y - newY) > carveOutRange / 2)
                clickAudio.PlayOneShot(clickSound);

            var carveOutLocalPosition = carveOut.localPosition;
            carveOutLocalPosition = new Vector3(carveOutLocalPosition.x, newY, carveOutLocalPosition.z);
            carveOut.localPosition = carveOutLocalPosition;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        for (int i = 0; i < carveOuts.Length; i++) {
            var carveOut = carveOuts[i];
            Gizmos.DrawSphere(carveOut.position, 0.1f);
        }
    }

    // programatically starting a spin, used for NPC's
    public void Spin() {
        spinSpeed = 3f;
        peakSpin = spinSpeed;
    }
}
