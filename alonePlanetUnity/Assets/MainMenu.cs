using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject _startGame;
    public GameObject _selectLevel;
    public GameObject _quit;

	void Start()
	{
        _startGame.GetComponent<Button>().onClick.AddListener(StartGame);
        _selectLevel.GetComponent<Button>().onClick.AddListener(SelectLevel);
        _quit.GetComponent<Button>().onClick.AddListener(Quit);
	}

    private void StartGame()
    {
        SceneManager.LoadScene("mainScene");
    }

    private void SelectLevel()
    {
        Debug.Log("SelectLevel");
    }

    private void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }

}
