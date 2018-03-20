using System;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour {

    public GameObject _contentPtr;
    public GameObject _buttonTemplate;
    public GameObject _backToMainMenuButton;

    void Start () {
        LoadLevels();
        _backToMainMenuButton.GetComponent<Button>().onClick.AddListener(delegate { BackToMainMenu(); });
	}

    private void ButtonClick(string level) {
        PlayerPrefs.SetString("level", level);
        SceneManager.LoadScene("mainScene");
    }

    private void BackToMainMenu() {
        SceneManager.LoadScene("mainMenu");
    }

    private void LoadLevels()
    {
        var content = FileReader.LoadFile("levelList.xml", this);
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
