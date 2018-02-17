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
    public Animator _animator;
    public GameObject _levelButtonsContainer;

    private bool _levelListShown = false;
    private List<string> _levelList = new List<string>();


	void Start()
	{
        _startGame.GetComponent<Button>().onClick.AddListener(StartGame);
        _selectLevel.GetComponent<Button>().onClick.AddListener(SelectLevel);
        _quit.GetComponent<Button>().onClick.AddListener(Quit);
        RectTransform rect = _levelButtonsContainer.GetComponent<RectTransform>();
        Debug.Log(rect.rect.width);
        _contentPtr.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100
                                                                                      ,100);

        _levelList.Add("1");
        _levelList.Add("2");
        _levelList.Add("3");
        _levelList.Add("4");
        _levelList.Add("5");
        _levelList.Add("6");
        _levelList.Add("7");
        _levelList.Add("8");
        //_levelList.Add("3");
        //_levelList.Add("4");
        //_levelList.Add("5");
        //_levelList.Add("6");
        //_levelList.Add("1");
        //_levelList.Add("2");
        //_levelList.Add("3");
        //_levelList.Add("4");
        //_levelList.Add("5");
        //_levelList.Add("6");

        foreach (var level in _levelList)
        {
            GameObject button = Instantiate(_buttonTemplate) as GameObject;
            button.GetComponent<Button>().onClick.AddListener(delegate { Button_Click("1"); });
            button.GetComponentInChildren<Text>().text = level;
            button.transform.SetParent(_contentPtr.transform, false);
            //button.transform.SetParent(_selectLevelView1.transform);
        }
        rect = (RectTransform)_levelButtonsContainer.transform;
        Debug.Log(rect.rect.width);
       
	}

    private void StartGame()
    {
        SceneManager.LoadScene("mainScene");
    }

    private void SelectLevel()
    {
        if (_levelListShown)
            _animator.SetTrigger("hide");
        else
            _animator.SetTrigger("show");
		_levelListShown = !_levelListShown;

        //_selectLevelView.GetComponent<Canvas>().enabled = !_selectLevelView.GetComponent<Canvas>().enabled;
    }

    private void Quit()
    {
        Application.Quit();
    }

    static public void Button_Click(string level)
    {
        PlayerPrefs.SetString("level", level);
        SceneManager.LoadScene("mainScene");
    }
}
