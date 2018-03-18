using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
