using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetonateScript : MonoBehaviour {
    private AudioScript audioScript;

    private bool detonated;
    private int timeLeft = 60;
    
    // Start is called before the first frame update
    void Start() {
        audioScript = GameObject.Find("Audio Source").GetComponent<AudioScript>();
    }

    // Update is called once per frame
    void Update() {
        if (detonated) {
            timeLeft--;
            if (timeLeft < 1) {
                GameObject.Destroy(gameObject);
            }
        }
    }

    public void TriggerDetonate () {
        detonated = true;

        audioScript.PlayDetonate();

        GetComponent<Animator>().SetBool("Detonated", true);
    }
}
