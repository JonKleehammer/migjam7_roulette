using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CylinderSpinner : MonoBehaviour {

    private bool isDragging = false;

    public Transform carveOutGroupT;
    [SerializeField]
    private Transform[] carveOuts;
    public float carveOutSpacing;
    public float carveOutRange = 3f;
    public float carveOutOffset;

    private float startRotation = 0;
    private float currentRotation = 0;
    private Vector3 startMousePos;
    private Vector3 lastMousePos;
    private Vector3 spinVelocity = Vector3.zero;
    private float spinSpeed = 0;
    private float spinDir = 1;
    public float spinFriction;
    private float peakSpin = 0;
    public float spinDuration;
    private float spinTime;
    public float minSpinSpeed = 0.01f;
    public float spinSpeedSoundThreshold;

    public AudioSource spinAudio;
    public AudioSource clickAudio;
    public AudioClip clickSound;
    
    // Start is called before the first frame update
    void Start() {
        carveOuts = carveOutGroupT.GetComponentsInChildren<Transform>();
        carveOuts = carveOuts.Skip(1).ToArray();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            isDragging = true;
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spinSpeed = 0;
            spinAudio.Stop();
        }
        else if (Input.GetMouseButton(0)) {
            lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var yMovement = lastMousePos.y - startMousePos.y;
            currentRotation = startRotation + yMovement;
        }
        else if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
            spinSpeed = lastMousePos.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            peakSpin = spinSpeed;
            spinTime = 0f;
            if (spinSpeed < 0) {
                spinSpeed *= -1;
                spinDir = 1;
            }
            else {
                spinDir = -1;
            }
        }

        if (spinSpeed > 0) {
            currentRotation += spinSpeed * spinDir;
            spinTime += Time.deltaTime;
            spinSpeed = Mathf.Lerp(spinSpeed, 0, spinTime / spinDuration);
            spinAudio.volume = Mathf.Clamp(spinSpeed, 0, 1);
            if (spinSpeed < minSpinSpeed)
                spinSpeed = 0f;
            print(spinSpeed);
        }
        
        for (int i = 0; i < carveOuts.Length; i++) {
            var carveOut = carveOuts[i];
            var newY = (currentRotation + carveOutRange * ((float) i / carveOuts.Length )) % carveOutRange;
            if (newY < 0)
                newY += carveOutRange;
            newY += carveOutOffset;
            
            if (!isDragging && spinSpeed > spinSpeedSoundThreshold && !spinAudio.isPlaying)
                spinAudio.Play();
            else if (spinSpeed <= spinSpeedSoundThreshold && Mathf.Abs(carveOut.localPosition.y - newY) > carveOutRange / 2)
                clickAudio.PlayOneShot(clickSound);
            
            carveOut.localPosition = new Vector3(carveOut.localPosition.x, newY, carveOut.localPosition.z);
        }


    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        for (int i = 0; i < carveOuts.Length; i++) {
            var carveOut = carveOuts[i];
            Gizmos.DrawSphere(carveOut.position, 0.1f);
        }
    }
}