using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CountDownController : MonoBehaviour
{

    public GameObject myTextObject;

    public GameManager myManager;

    public Text myText;
    // Start is called before the first frame update
    void Start()
    {
        //myText = myTextObject.GetComponent<Text>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void countDown(){
        StartCoroutine(actualCountDown());
    }

    public IEnumerator actualCountDown(){
        //  TODO: ADD SFX
        myTextObject.SetActive(true);
        myText.text = "3!";
        yield return new WaitForSeconds(1f);
        myText.text = "2!";
        yield return new WaitForSeconds(1f);
        myText.text = "1!";
        yield return new WaitForSeconds(1f);
        myText.text = "GO!";
        yield return new WaitForSeconds(1f);
        myTextObject.SetActive(false);
        myManager.gameStart();

        // TODO: CALL MANAGER TO MOVE ON
    }
}
