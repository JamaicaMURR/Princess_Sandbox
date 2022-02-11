using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string previousSceneName;
    public string thisSceneName;
    public string nextSceneName;

    public UnityEvent OnSwitch;

    public void SwitchToNextScene()
    {
        OnSwitch?.Invoke();
        SceneManager.LoadScene(nextSceneName); 
    }

    public void ReloadScene()
    {
        OnSwitch?.Invoke();
        SceneManager.LoadScene(thisSceneName);
    }

    public void SwitchToPrevious() 
    {
        OnSwitch?.Invoke();
        SceneManager.LoadScene(previousSceneName); 
    }
}
