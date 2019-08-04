using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetonateScript : MonoBehaviour {
    private bool detonated;
    private int timeLeft = 60;
    
    // Start is called before the first frame update
    void Start() {
        
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
        detonated = true;;

        GetComponent<Animator>().SetBool("Detonated", true);
    }
}
