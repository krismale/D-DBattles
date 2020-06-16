using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public int CragmawHideoutID;
    public int RedbrandHideoutID;
    public void OpenCragmawHideout()
    {
        SceneManager.LoadScene(CragmawHideoutID);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void OpenRedbrandHideout()
    {
        SceneManager.LoadScene(RedbrandHideoutID);
    }
}
