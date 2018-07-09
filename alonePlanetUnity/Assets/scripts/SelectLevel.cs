using System;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour {

    public GameObject _contentPtr;
    public GameObject _buttonTemplate;
    public GameObject _backToMainMenuButton;

    void Start () {
        FileReader.LoadFile("levelList.xml", this, LevelListIsReady);
        _backToMainMenuButton.GetComponent<Button>().onClick.AddListener(delegate { BackToMainMenu(); });
	}

    public void LevelListIsReady(string content)
    {
        LoadLevels(content);
    }

    private void ButtonClick(string level) {
        PlayerPrefs.SetString(GameConstants.CurrentLevel, level);
        PlayerPrefs.SetInt(GameConstants.CurrentLevelIsCompleted, 0);
        SceneManager.LoadScene("mainScene");
    }

    private void BackToMainMenu() {
        SceneManager.LoadScene("mainMenu");
    }

    private void LoadLevels(string content)
    {
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(content);
        XmlNodeList levels = xmldoc.SelectNodes("/levels/level");
        foreach (XmlNode level in levels)
        {
            GameObject button = Instantiate(_buttonTemplate) as GameObject;
            button.GetComponent<Button>().onClick.AddListener(delegate { ButtonClick(level.Attributes.GetNamedItem("file").Value); });
            button.GetComponentInChildren<Text>().text = level.Attributes.GetNamedItem("name").Value;
            button.transform.SetParent(_contentPtr.transform, false);
            button.GetComponent<Button>().interactable = Convert.ToBoolean(level.Attributes.GetNamedItem("enabled").Value);
        }
    }
}
