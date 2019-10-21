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

    enum State {Scaning, writingPunishment, beforeTurntable, turntableStart, codingGame}


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
    // Start is called before the first frame update
    void Start()
    {
        myState = State.Scaning;
        showPanelwithAnim(instructionPanel,popupClip);
        instructionText.text = scanInstruction;
    }

    // Update is called once per frame
    void Update()
    {
        switch (myState)
        {
            case State.Scaning:
                break;
            case State.writingPunishment:
                bool everyoneReady = true;

                // check every one has submitted the punishment
                foreach (Player p in PhotonNetwork.PlayerList) {
                    if(!(bool)p.CustomProperties["hasSubmittedPunishment"]){
                        everyoneReady = false;
                    }
                    else{
                        // if the local player has submitted the punishment but others don't, show the waiting panel
                        if (p.IsLocal){
                            waitingPanel.SetActive(true);
                        }
                    }
                }

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
                        }
                    }
                }
                break;

            case State.turntableStart:
                break;
            default:
                break;
        }
    }

    public void finishScanning(){
        myState = State.writingPunishment;
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
        myState = State.beforeTurntable;
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
    }

    public void drawTurntable(){
        int choice = myTurntableController.selectNum();
        myTurntableController.drawTurntable(choice);
        ExitGames.Client.Photon.Hashtable costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "hasDrawedTurntable",true },
					{ "turntableChoice",choice },	
				};
        PhotonNetwork.SetPlayerCustomProperties (costomProperties);
        myState = State.turntableStart;
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

    public void onClickGotit(){
        switch (myState){
            default:
                break;
        }

        showPanelwithAnim(instructionPanel,disappearClip);
    }
}
