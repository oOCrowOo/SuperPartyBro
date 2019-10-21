using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cokeshakerController : MonoBehaviour
{

    const float SHAKEAMOUNTLIMIT = 100f;

    public AudioClip sprayClip;
    public GameObject sprayedCokeParticle;

    public GameObject canModel;

    private float shakeAmountTotal = 0f;

    private Vector3 acceleration;

    private bool duringShaking = true;

    private bool hasSprayed = false;

    public GameManager myManager;

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(spray());
        acceleration = Input.acceleration;
    }

    void OnEnable(){
        duringShaking = true;
        hasSprayed = false;
    }

    // Update is called once per frame
    void Update()
    {   
        //return;
        if(duringShaking){
            shake();
        }
        
        if (shakeAmountTotal >= SHAKEAMOUNTLIMIT && !hasSprayed){
            StartCoroutine(spray());
            hasSprayed = true;
            duringShaking = false;

            // finished the game locally
            
            
        }
        
    }

    private void reset(){
        shakeAmountTotal = 0;

    }
    public void shake(){
        // Debug.Log(Input.acceleration);
        float yDiff = (Input.acceleration.y - acceleration.y) / 10f;
        float yDiffabs = Mathf.Abs(yDiff);
        shakeAmountTotal += yDiffabs;
        Vector3 oldPosition = canModel.transform.localPosition;
        Vector3 newPosition = new Vector3(oldPosition.x,oldPosition.y+yDiff,oldPosition.z);
        canModel.transform.localPosition = newPosition;
        acceleration = Input.acceleration;
    }

    public IEnumerator spray(){
        canModel.transform.localPosition = new Vector3(0,-0.2f,1);
        sprayedCokeParticle.SetActive(true);
        this.GetComponent<AudioSource>().clip = sprayClip;
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(sprayClip.length);
        sprayedCokeParticle.SetActive(false);
        reset();
        myManager.gameOver();
    }
}
