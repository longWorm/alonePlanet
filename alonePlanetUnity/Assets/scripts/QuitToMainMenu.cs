using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitToMainMenu : MonoBehaviour
{
    public GameObject _quit;
	
	void Start()
	{
        _quit.GetComponent<Button>().onClick.AddListener(Quit);
	}

	// Update is called once per frame
	void Quit()
	{
        SceneManager.LoadScene("mainMenu");
	}
}
