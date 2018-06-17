using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Walkthrough : MonoBehaviour
{
    public static Walkthrough Instance;

    public bool showWalkthrough;
    public Transform[] steps;
    public Image[] stepFillBars;
    public Transform targets;
    int _targets = 5;

    public bool IsRunning { get { return _stepIdx >= 0; } }
    public bool ShootLocked { get { return IsRunning && _stepIdx < 1; } }

    int _stepIdx = -1;


    private void Awake()
    {
        Instance = this;

#if !UNITY_EDITOR
        showWalkthrough = !PlayerPrefs.HasKey("DoneTutorial");
#endif

        if (showWalkthrough)
            StartWalkthrough();
        else
            GameManager.Instance.birdSpawner.enabled = true;
    }

    public void StartWalkthrough()
    {
        _stepIdx = 0;
        UpdateStep();
    }

    public void NextStep()
    {
        _stepIdx++;
        UpdateStep();
    }

    void UpdateStep()
    {
        for (int i = 0; i < steps.Length; i++)
            steps[i].gameObject.SetActive(i == _stepIdx);

        targets.gameObject.SetActive(_stepIdx == 2);

        if (_stepIdx == steps.Length - 1)
        {
            PlayerPrefs.SetInt("DoneTutorial", 1);
            StartCoroutine(StartGameInSeconds(3f));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from">0 is camera movement, 1 is gun</param>
    public void FillStepBar(float value, int from)
    {
        if (_stepIdx == from)
        {
            stepFillBars[_stepIdx].fillAmount = Mathf.Clamp01(stepFillBars[_stepIdx].fillAmount + value);
            if (stepFillBars[_stepIdx].fillAmount == 1)
                NextStep();
        }
    }

    public void ShotTarget()
    {
        _targets--;

        if (_targets == 0)
            NextStep();
        else
            steps[_stepIdx].GetComponentInChildren<Text>().text = "shoot " + _targets + " targets";
    }

    IEnumerator StartGameInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // clear the walkthrough state
        _stepIdx = -1;
        UpdateStep();

        // start the bird spawner
        GameManager.Instance.birdSpawner.enabled = true;
    }
}
