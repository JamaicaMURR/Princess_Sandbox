using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string previousSceneName;
    public string currentSceneName;
    public string nextSceneName;

    public UnityEvent OnSwitch;

    public void SwitchToNext()
    {
        OnSwitch?.Invoke();
        SceneManager.LoadScene(nextSceneName); 
    }

    public void ReloadScene()
    {
        OnSwitch?.Invoke();
        SceneManager.LoadScene(currentSceneName);
    }

    public void SwitchToPrevious() 
    {
        OnSwitch?.Invoke();
        SceneManager.LoadScene(previousSceneName); 
    }
}
