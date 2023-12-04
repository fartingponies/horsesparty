using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Button englishButton, frenchButton, clearProgressButton;
    public Material languageOn;

    void Start()
    {
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
        {
            EnglishButtonClicked();
        }
        else
        {
            FrenchButtonClicked();
        }

    }

    public void ClearProgressClicked()
    {
        PlayerPrefs.SetInt("currentMission", 0);
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.SetInt("TutorialDone" + i, 0);
        }
    }

    public void EnglishButtonClicked()
    {
        englishButton.GetComponent<Image>().material = languageOn;
        frenchButton.GetComponent<Image>().material = null;

        englishButton.GetComponent<Image>().material.SetFloat("_Outline", 1f);
        englishButton.GetComponent<Image>().material.SetColor("_OutlineColor", Color.red);
        englishButton.GetComponent<Image>().material.SetFloat("_OutlineSize", 4f);

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
    }

    public void FrenchButtonClicked()
    {
        frenchButton.GetComponent<Image>().material = languageOn;
        englishButton.GetComponent<Image>().material = null;

        frenchButton.GetComponent<Image>().material.SetFloat("_Outline", 1f);
        frenchButton.GetComponent<Image>().material.SetColor("_OutlineColor", Color.red);
        frenchButton.GetComponent<Image>().material.SetFloat("_OutlineSize", 4f);

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    }
}
