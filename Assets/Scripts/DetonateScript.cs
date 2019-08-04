using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetonateScript : MonoBehaviour {
    private bool detonated;
    private Time detonateTime;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (detonated) {
            if (detonateTime + 2 < Time.time) {
                GameObject.Destroy(this);
            }
        }
    }

    public TriggerDetonate () {
        detonated = true;
        detonateTime = Time.time;

        GetComponent<Animator>().SetBool("Detonated", true);
    }
}
