using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text Title;
    public Text Desc;

    // Start is called before the first frame update
    void Start()
    {
        EnableText(false);

        GameEvents.current.onSetTankPositionStart += onSetTankPositionStart;
        GameEvents.current.onSetTankPositionEnd += onSetTankPositionEnd;
        GameEvents.current.onTankReadyFireStart += onTankReadyFireStart;
        GameEvents.current.onTankReadyFireEnd += onTankReadyFireEnd;
        GameEvents.current.onGameOver += onGameOver;
    }

    private void onSetTankPositionStart(int id)
    {
        EnableText(true);

        if (id == 1)
        {
            Title.text = "PLAYER";
            Desc.text = "click to move tank";
        }
        else
        {
            Title.text = "OPPONENT";
            Desc.text = "thinking, please wait...";
        }
    }
    private void onSetTankPositionEnd(int id)
    {
        EnableText(false);
    }
    private void onTankReadyFireStart(int id)
    {
        EnableText(true);

        if (id == 1)
        {
            Title.text = "PLAYER";
            Desc.text = "hold space to fire";
        }
        else
        {
            Title.text = "OPPONENT";
            Desc.text = "firing, please wait...";
        }
    }
    private void onTankReadyFireEnd(int id)
    {
        EnableText(false);
    }

    private void onGameOver(bool won)
    {
        EnableText(true);

        if (won)
        {
            Title.text = "VICTORY";
            Desc.text = "";
        }
        else
        {
            Title.text = "DEFEAT";
            Desc.text = "";
        }
    }

    // Update is called once per frame
    void EnableText(bool enable)
    {
        gameObject.SetActive(enable);
    }

    public void onDestroy()
    {
        GameEvents.current.onSetTankPositionStart -= onSetTankPositionStart;
        GameEvents.current.onSetTankPositionEnd -= onSetTankPositionEnd;
        GameEvents.current.onTankReadyFireStart -= onTankReadyFireStart;
        GameEvents.current.onTankReadyFireEnd -= onTankReadyFireEnd;
        GameEvents.current.onGameOver -= onGameOver;
    }
}
