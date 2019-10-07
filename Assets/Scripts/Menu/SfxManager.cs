using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void playSFX(AudioClip clip){
        this.GetComponent<AudioSource>().clip = clip;
        this.GetComponent<AudioSource>().Play();
    }
}
