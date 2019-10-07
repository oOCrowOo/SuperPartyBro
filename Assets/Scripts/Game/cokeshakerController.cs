using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cokeshakerController : MonoBehaviour
{


    public AudioClip sprayClip;
    public GameObject sprayedCokeParticle;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spray());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator spray(){
        sprayedCokeParticle.SetActive(true);
        this.GetComponent<AudioSource>().clip = sprayClip;
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(sprayClip.length);
        sprayedCokeParticle.SetActive(false);
    }
}
