using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public GameObject _startGame;
    public GameObject _selectLevel;
    public GameObject _selectLevelView;
    public GameObject _quit;
    public GameObject _buttonTemplate;
    public GameObject _contentPtr;

    private List<string> _levelList = new List<string>();


	void Start()
	{
        _startGame.GetComponent<Button>().onClick.AddListener(StartGame);
        _selectLevel.GetComponent<Button>().onClick.AddListener(SelectLevel);
        _quit.GetComponent<Button>().onClick.AddListener(Quit);
        _selectLevelView.GetComponent<Canvas>().enabled = false;

        _levelList.Add("1");
        _levelList.Add("2");
        _levelList.Add("3");
        _levelList.Add("4");
        _levelList.Add("5");
        foreach (var level in _levelList)
        {
            GameObject button = Instantiate(_buttonTemplate) as GameObject;
            button.GetComponent<Button>().onClick.AddListener(delegate { Button_Click(level); });
            button.GetComponentInChildren<Text>().text = level;
            button.transform.SetParent(_contentPtr.transform);
        }
	}

    private void StartGame()
    {
        SceneManager.LoadScene("mainScene");
    }

    private void SelectLevel()
    {
        _selectLevelView.GetComponent<Canvas>().enabled = !_selectLevelView.GetComponent<Canvas>().enabled;
    }

    private void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    static public void Button_Click(string level)
    {
        PlayerPrefs.SetString("level", level);
        SceneManager.LoadScene("mainScene");
    }
}
