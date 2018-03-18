using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SelectLevel : MonoBehaviour {

    public GameObject _contentPtr;
    public GameObject _buttonTemplate;

    void Start () {
        List<string> levelList = new List<string>();

        levelList.Add("1");
        levelList.Add("2");
        levelList.Add("3");
        levelList.Add("4");
        levelList.Add("5");
        levelList.Add("6");
        levelList.Add("7");
        levelList.Add("8");
    
        foreach (var level in levelList) {
            GameObject button = Instantiate(_buttonTemplate) as GameObject;
            button.GetComponent<Button>().onClick.AddListener(delegate { ButtonClick(level); });
            button.GetComponentInChildren<Text>().text = level;
            button.transform.SetParent(_contentPtr.transform, false);
        }

	}

    private void ButtonClick(string level) {
        Debug.Log(level);
        Debug.Log("clicked");
        PlayerPrefs.SetString("level", level);
        SceneManager.LoadScene("mainScene");
    }
}
