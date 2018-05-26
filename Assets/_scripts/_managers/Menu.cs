using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject[] buttons;


    private void Awake()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            float delay = i * 0.25f;
            LeanTween.scale(buttons[i], Vector3.one * 1.05f, 1f).setEase(LeanTweenType.punch).setLoopType(LeanTweenType.linear).setDelay(delay);
        }
    }

    public void EnterDomeButtonPressed()
    {
        SceneManager.LoadScene("bird attack");
    }
        
}
