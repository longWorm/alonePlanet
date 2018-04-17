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
        if (PlayerPrefs.GetInt(GameConstants.CurrentLevelIsCompleted) != 0)
            MarkCurrentLevelAsCompleted();
        
        LoadLevels();
        _backToMainMenuButton.GetComponent<Button>().onClick.AddListener(delegate { BackToMainMenu(); });
	}

    private void ButtonClick(string level) {
        PlayerPrefs.SetString(GameConstants.CurrentLevel, level);
        PlayerPrefs.SetInt(GameConstants.CurrentLevelIsCompleted, 0);
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

    private void MarkCurrentLevelAsCompleted()
    {
        if (PlayerPrefs.GetString(GameConstants.CurrentLevel).Length == 0)
            return;
        
        var content = FileReader.LoadFile("levelList.xml", this);
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(content);
        var currentLevel = Convert.ToInt16(PlayerPrefs.GetString(GameConstants.CurrentLevel));
        currentLevel++;
        var node = xmldoc.SelectSingleNode("/levels/level[@file=\"" + Convert.ToString(currentLevel) + "\"]");
        if (node == null)
            return;
        
        node.Attributes["enabled"].Value = "true";

        var root = xmldoc.SelectSingleNode("/levels");
        if (root.Attributes["currentLevel"] == null)
        {
            XmlAttribute newAttribute = xmldoc.CreateAttribute("currentLevel");
            newAttribute.Value = Convert.ToString(currentLevel);
            root.Attributes.Append(newAttribute);
        }
        else
            root.Attributes["currentLevel"].Value = Convert.ToString(currentLevel);

        FileReader.WriteToFile("levelList.xml", xmldoc.OuterXml);
    }
}
