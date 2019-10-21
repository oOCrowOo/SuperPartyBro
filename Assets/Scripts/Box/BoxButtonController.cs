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

    private void Awake()
    {
        SubmitButton = GameObject.Find("Submit").GetComponent<Button>();
    }

    public void disablePanel()
    {
        PlayerInput = GameObject.Find("InputField").GetComponent<InputField>();
        Debug.Log(PlayerInput.text);
        PanelAnim.Play();
       //PaperPanel.SetActive(false);
    }

}
