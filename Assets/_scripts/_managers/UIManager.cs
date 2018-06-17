using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class UIManager : MonoBehaviour
{
    public GameObject settingsButton;
    public GameObject settingsPanel;
    public Text sensitivityValue;
    public Slider sensitivitySlider;
    public TouchPad touchPad;


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
}
