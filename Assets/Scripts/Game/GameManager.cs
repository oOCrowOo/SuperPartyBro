using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{

    enum State {Scaning, writingPunishment, beforeTurntable, turntableStart, codingGame}


    private State myState;

    public GameObject myTurntable;

    public turntableController myTurntableController;

    public GameObject turntableButton;

    public AnimationClip popupClip;
    public AnimationClip rotateClip;

    public GameObject waitingPanel;
    // Start is called before the first frame update
    void Start()
    {
        myState = State.Scaning;
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
                foreach (Player p in PhotonNetwork.PlayerList) {
                    
                    if(!(bool)p.CustomProperties["hasSubmittedPunishment"]){
                        everyoneReady = false;
                    }
                    else{
                        if (p.IsLocal){
                            waitingPanel.SetActive(true);
                        }
                    }
                }
                if (everyoneReady){
                    switchToBeforeTurntable();
                }
                break;
            case State.beforeTurntable:

                if (PhotonNetwork.LocalPlayer.IsMasterClient){
                    return;
                }

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
        Debug.Log("haahahahahaha");
        // TODO: Enable the turnable
        // myTurntable.SetActive(true);
        ExitGames.Client.Photon.Hashtable costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "hasDrawedTurntable",false },	// whether the player has drawed the turntable
					{ "turntableChoice",-1 },	// the choice
				};
        PhotonNetwork.SetPlayerCustomProperties (costomProperties);
        if (PhotonNetwork.IsMasterClient){
            // TODO: Enable the rotate button
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
					{ "hasDrawedTurntable",true },	// whether the player has submitted his punishment
					{ "turntableChoice",choice },	// the content of punishment
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
}
