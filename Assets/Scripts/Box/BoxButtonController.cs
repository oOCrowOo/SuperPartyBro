using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxButtonController : MonoBehaviour
{
    public GameObject PaperPanel;
    private Button SubmitButton;
    private InputField PlayerInput;
    public Animation PanelAnim;

    public GameManager myManger;

    private void Awake()
    {
        SubmitButton = GameObject.Find("Submit").GetComponent<Button>();
    }

    public void disablePanel()
    {
        PlayerInput = GameObject.Find("InputField").GetComponent<InputField>();
        if (PlayerInput.text != ""){
            PanelAnim.Play();
            myManger.submitPunishment(PlayerInput.text);
        }
    }

}
