using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {
    public AudioSource audioSource;

    public AudioClip spawn;
    public AudioClip death;
    public AudioClip move;
    public AudioClip wall;
    public AudioClip detonate;

    // Start is called before the first frame update
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        
    }

    public void PlaySpawn () {
        audioSource.PlayOneShot(spawn);
    }

    public void PlayMove () {
        audioSource.PlayOneShot(move);
    }

    public void PlayDeath () {
        audioSource.PlayOneShot(death);
    }

    public void PlayWall () {
        audioSource.PlayOneShot(wall);
    }

    public void PlayDetonate () {
        audioSource.PlayOneShot(detonate);
    }
}
