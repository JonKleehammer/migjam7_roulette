using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherScreenManager : MonoBehaviour {
    
    private float time;
    public float duration;
    public Image overlay;

    void Update()
    {
        time += Time.deltaTime;
        var a = Mathf.Cos(time / duration); 
        overlay.color = new Color(0, 0, 0, a);
    }
}
