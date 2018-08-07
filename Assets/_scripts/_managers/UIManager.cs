using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class UIManager : MonoBehaviour
{
    public Canvas worldCanvas;
    public GameObject beaksPanel;
    public Text beaksHeaderText;
    public Text beaksValueText;

    public GameObject settingsButton;
    public GameObject settingsPanel;
    public Text sensitivityValue;
    public Slider sensitivitySlider;
    public TouchPad touchPad;
    public Animator waveCompleteAnim;
    public Text scoreText, addingScoreText;
    public int scoreIncrements;
    int _addingScore, _totalScore;


    private void Start()
    {
        float sens = PlayerPrefs.GetFloat("sensitivity", 0.5f);
        sensitivitySlider.value = sens;
        SetSensitivity(sens);
    }

    public void SettingsButtonPressed()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        if (!settingsPanel.activeSelf)
        {
            // when turning off settings panel, update values
            SetSensitivity(sensitivitySlider.value);
        }
        settingsButton.SetActive(!settingsPanel.activeSelf);
        Time.timeScale = settingsPanel.activeSelf ? 0f : 1f;
        AudioListener.pause = settingsPanel.activeSelf;
    }

    public void SliderValueChanged()
    {
        sensitivityValue.text = sensitivitySlider.value.ToString("F1");
    }

    void SetSensitivity(float value)
    {
        float scaledValue = Mathf.Lerp(GameManager.Instance.sensitivityBounds.x, GameManager.Instance.sensitivityBounds.y, value);
        touchPad.Xsensitivity = scaledValue;
        touchPad.Ysensitivity = scaledValue;
        sensitivityValue.text = value.ToString("F1");
        PlayerPrefs.SetFloat("sensitivity", value);
    }

    public void MenuButtonPressed()
    {
        SceneManager.LoadScene("title");
    }

    public void ShowWaveCompleteAnim()
    {
        waveCompleteAnim.gameObject.SetActive(true);
        waveCompleteAnim.Play("waveComplete_show_anim", 0, 0f);
    }

    public void GiveBeaks(int beaks) {
        StartCoroutine(GiveBeaksCoroutine(beaks));
    }

    IEnumerator GiveBeaksCoroutine(int beaks)
    {
        beaksHeaderText.gameObject.SetActive(true);
        beaksValueText.gameObject.SetActive(false);
        beaksPanel.SetActive(true);
        worldCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        beaksValueText.text = beaks + " beaks!";
        beaksValueText.gameObject.SetActive(true);
    }

    public void AddScore(int score, int total)
    {
        StopCoroutine("HideAddedScore");
        _totalScore = total;
        if (addingScoreText.gameObject.activeSelf)
        {
            // add score to already showing add score
            int current = int.Parse(addingScoreText.text.Substring(1));
            addingScoreText.text = "+" + (current + score).ToString();
        }
        else
        {
            // show new add score
            addingScoreText.text = "+" + score.ToString();
        }
        addingScoreText.gameObject.SetActive(true);
        StartCoroutine("HideAddedScore");
    }

    IEnumerator HideAddedScore()
    {
        float current = int.Parse(scoreText.text);
        float increment = (float)(_totalScore - current) / (float)scoreIncrements;
        for (int i = 0; i < scoreIncrements; i++)
        {
            current += increment;
            scoreText.text = Mathf.RoundToInt(current + increment).ToString();
            yield return null;
        }
        scoreText.text = _totalScore.ToString();

        yield return new WaitForSeconds(1f);
        addingScoreText.gameObject.SetActive(false);
    }
}
