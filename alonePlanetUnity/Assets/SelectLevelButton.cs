using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SelectLevelButton : MonoBehaviour
{
    public GameObject _buttonTemplate;
    public GameObject _contentPtr;
	private List<string> _levelList = new List<string>();

    void Start()
    {
       Debug.Log("start");
        _levelList.Add("1");
        _levelList.Add("2");
        _levelList.Add("3");
        _levelList.Add("4");
        _levelList.Add("5");
        foreach(var level in _levelList)
        {
            GameObject button = Instantiate(_buttonTemplate) as GameObject;
            button.GetComponent<Button>().onClick.AddListener(delegate { Button_Click(level); });
            button.transform.SetParent(_contentPtr.transform);
        }
    }

    static public void Button_Click(string level)
    {
        PlayerPrefs.SetString("level", level);
        SceneManager.LoadScene("mainScene");
    }
}
