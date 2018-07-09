using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml;

public class MainMenu : MonoBehaviour
{
    public GameObject _continueGame;
    public GameObject _selectLevel;
	public GameObject _quit;

    void Start()
	{
        _continueGame.GetComponent<Button>().onClick.AddListener(ContinueGame);
        _selectLevel.GetComponent<Button>().onClick.AddListener(SelectLevel);
        _quit.GetComponent<Button>().onClick.AddListener(Quit);
        PlayerPrefs.SetInt(GameConstants.CurrentLevelIsCompleted, 0);

        FileReader.LoadFile("levelList.xml", this, LevelListIsReady);
    }

    public void LevelListIsReady(string content)
    {
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(content);
        var root = xmldoc.SelectSingleNode("/levels");
        if (PlayerPrefs.GetString(GameConstants.CurrentLevel, "") == "")
        {
            if (root.Attributes["currentLevel"] != null)
                PlayerPrefs.SetString(GameConstants.CurrentLevel, root.Attributes["currentLevel"].Value);
            else
                PlayerPrefs.SetString(GameConstants.CurrentLevel, "");
        }
    }

    private void ContinueGame()
    {
        SceneManager.LoadScene("mainScene");
    }

    private void SelectLevel()
    {
        SceneManager.LoadScene("selectLevel");
    }

    private void Quit()
    {
        Application.Quit();
    }
}
