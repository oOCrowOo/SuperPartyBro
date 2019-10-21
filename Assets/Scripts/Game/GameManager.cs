using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{

    const string scanInstruction = @"
    Welcome to the super party bro world. This is the ar world linked with your friends. You will play games with your friends and the loser will take the punishments! Please use your phone's camera to scan a smooth plane until the grids shows up. Then click on the grids to continue.
    ";

    const string punishmentInstruction = @"Please write down the punishment then click the submit button. The loser of the game will randomly pick one of the punishments.";

    const string cokeShakingInstruction = @"Welcome to the game Cola Shaker! After the count down, please shake your phone as crazy as possible! The player who let the cola spray out first will be the winner!";

    enum State {Scaning, writingPunishment, beforeTurntable, turntableStart, turntableFinish,
     pouring, drinking, beforeCokeShaking,cokeShaking ,beforeCodingGame}


    private State myState;

    public GameObject myTurntable;

    public turntableController myTurntableController;

    public GameObject turntableButton;

    public AnimationClip popupClip;
    public AnimationClip rotateClip;

    public GameObject waitingPanel;

    public GameObject instructionPanel;
    public Text instructionText;

    public AnimationClip disappearClip;

    public GameObject countDownPanel;

    private CountDownController countdownController;

    public ARController myArController;

    private GameObject ARCamera;

    private int chosenPlayerIndex = 0;

    public GameObject cokeCan;
    // Start is called before the first frame update
    void Start()
    {
        ARCamera = myArController.ARCamera;
        countdownController = countDownPanel.GetComponent<CountDownController>();
        myState = State.Scaning;
        showPanelwithAnim(instructionPanel,popupClip);
        instructionText.text = scanInstruction;
        setProperty("ready",false);
    }

    private bool everyoneCheck(string key){
        bool returnEntry = true;

        // check every one has submitted the punishment
        foreach (Player p in PhotonNetwork.PlayerList) {
            if(!(bool)p.CustomProperties[key]){
                returnEntry = false;
                Debug.Log(p);
            }
            else{
                // if the local player has submitted the punishment but others don't, show the waiting panel
                if (p.IsLocal){
                    waitingPanel.SetActive(true);
                }
            }
        }
        return returnEntry;
    }

    // Update is called once per frame
    void Update()
    {
        switch (myState)
        {
            case State.Scaning:
                break;
            case State.writingPunishment:
                bool everyoneReady = everyoneCheck("hasSubmittedPunishment");

                // // check every one has submitted the punishment
                // foreach (Player p in PhotonNetwork.PlayerList) {
                //     if(!(bool)p.CustomProperties["hasSubmittedPunishment"]){
                //         everyoneReady = false;
                //     }
                //     else{
                //         // if the local player has submitted the punishment but others don't, show the waiting panel
                //         if (p.IsLocal){
                //             waitingPanel.SetActive(true);
                //         }
                //     }
                // }

                // if everyone has submitted the punishment, move to the next stage
                if (everyoneReady){
                    switchToBeforeTurntable();
                }
                break;
            case State.beforeTurntable:

                // master will click the button, no need to wait
                if (PhotonNetwork.LocalPlayer.IsMasterClient){
                    return;
                }

                // check if the master draw the turntable
                foreach (Player p in PhotonNetwork.PlayerList) {
                    if(p.IsMasterClient){
                        if((bool)p.CustomProperties["hasDrawedTurntable"]){
                            myTurntableController.drawTurntable((int)p.CustomProperties["turntableChoice"]);
                            myState = State.turntableStart;
                            chosenPlayerIndex = (int)p.CustomProperties["playerChoice"];
                        }
                    }
                }
                break;

            case State.turntableStart:
                break;
            case State.turntableFinish:
                break;
            case State.pouring:
            case State.drinking:
                everyoneReady = everyoneCheck("ready");
                // Event finished, back to turntable
                // Debug.Log("=======================");
                // Debug.Log(everyoneReady);
                if (everyoneReady){
                    switchToBeforeTurntable();
                    showPanelwithAnim(myTurntable,popupClip);
                }
                break;
            case State.beforeCokeShaking:
                everyoneReady = everyoneCheck("ready");
                if (everyoneReady){
                    myState = State.cokeShaking;
                    countDownPanel.SetActive(true);
                    waitingPanel.SetActive(false);
                    countdownController.countDown();

                }
                break;
            default:
                break;
        }
    }

    public void gameStart(){
        countDownPanel.SetActive(false);
        switch(myState){
            case State.cokeShaking:
                cokeCan.SetActive(true);
                cokeCan.transform.parent = ARCamera.transform; 
                cokeCan.transform.localPosition = new Vector3(0,-0.2f,1);
                cokeCan.transform.localRotation = Quaternion.identity;
                break;
        }
    }

    

    public void finishScanning(){
        myState = State.writingPunishment;
        showPanelwithAnim(instructionPanel,popupClip);
        instructionText.text = punishmentInstruction;
    }

    public void submitPunishment(string punishment){
        //PhotonNetwork.LocalPlayer.
        ExitGames.Client.Photon.Hashtable costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "hasSubmittedPunishment",true },	// whether the player has submitted his punishment
					{ "punishment",punishment },	// the content of punishment
				};
        PhotonNetwork.SetPlayerCustomProperties (costomProperties);
    }

    public void switchToBeforeTurntable(){
        //setProperty("ready",false);
        
        waitingPanel.SetActive(false);
        ExitGames.Client.Photon.Hashtable costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "hasDrawedTurntable",false },	// whether the player has drawed the turntable
					{ "turntableChoice",-1 },	// the choice
				};
        PhotonNetwork.SetPlayerCustomProperties (costomProperties);
        if (PhotonNetwork.IsMasterClient){
            turntableButton.SetActive(true);
        }
        else{
            turntableButton.SetActive(false);
        }
        myState = State.beforeTurntable;
    }

    public void drawTurntable(){
        int choice = myTurntableController.selectNum();
        int playerChoice = Random.Range(0,PhotonNetwork.PlayerList.Length - 1);
        myTurntableController.drawTurntable(choice);
        ExitGames.Client.Photon.Hashtable costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "hasDrawedTurntable",true },
					{ "turntableChoice",choice },	
                    { "playerChoice",playerChoice },
				};
        PhotonNetwork.SetPlayerCustomProperties (costomProperties);
        myState = State.turntableStart;
        turntableButton.SetActive(false);
    }

    public IEnumerator switchPanel(GameObject oldPanel, GameObject newPanel, AnimationClip leaveClip, AnimationClip inClip)
    {
        // Play the fade out animation for the old panel
        Animation fadeoutAnimation = oldPanel.GetComponent<Animation>();
        float duration = leaveClip.length;
        fadeoutAnimation.clip = leaveClip;
        fadeoutAnimation.Play();
        // after the seconds of the clip length
        yield return new WaitForSeconds(duration);

        // Play the move in animation for the new panel
        newPanel.SetActive(true);
        Animation moveinAnimation = newPanel.GetComponent<Animation>();
        moveinAnimation.clip = inClip;
        moveinAnimation.Play();
        oldPanel.SetActive(false);
        oldPanel.GetComponent<Transform>().localPosition = new Vector3(0,0,0);
    }

    public void showPanelwithAnim(GameObject panel, AnimationClip clip){
         // Play the move in animation for the new panel
        panel.SetActive(true);
        Animation moveinAnimation = panel.GetComponent<Animation>();
        moveinAnimation.clip = clip;
        moveinAnimation.Play();
    }

    public void finishTurntable(int selectNum){
        myState = State.turntableFinish;
        setProperty("ready",false);
        setProperty("hasDrawedTurntable",false);
        StartCoroutine(switchPanel(myTurntable,instructionPanel,disappearClip,popupClip));
        switch(selectNum){
            // crazy programmer game
            case 0:
                break;
            // cola shaking game
            case 1:
                myState = State.beforeCokeShaking;
                instructionText.text = cokeShakingInstruction;
                break;
            // pouring drinks into cup
            case 2:
                myState = State.pouring;
                //showPanelwithAnim(instructionPanel,popupClip);
                instructionText.text = "Event determined! Player " + PhotonNetwork.PlayerList[chosenPlayerIndex].NickName + ", please choose one of whatever drinks on the table and pour it into the cup!";
                break;
            // drink everything inside the cup
            case 3:
                myState = State.drinking;
                instructionText.text = "Event determined! Player " + PhotonNetwork.PlayerList[chosenPlayerIndex].NickName + ", please drink up this the cup of whatever! :(";
                break;
            default:
                break;
        }
    }

    private void setProperty(string key, object value){
         ExitGames.Client.Photon.Hashtable costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ key,value }
				};
        PhotonNetwork.SetPlayerCustomProperties (costomProperties);
    }

    public void onClickGotit(){
        switch (myState){
            case State.pouring:
            case State.drinking:
            case State.beforeCokeShaking:
                setProperty("ready",true);
                //showPanelwithAnim(waitingPanel,popupClip);
                break;
            default:
                break;
        }

        showPanelwithAnim(instructionPanel,disappearClip);
    }
}
