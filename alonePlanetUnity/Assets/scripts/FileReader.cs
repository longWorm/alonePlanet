using UnityEngine;
using System;
using System.Collections;

public class FileReader
{
#if UNITY_ANDROID
    private static IEnumerator WaitForWWW(WWW www)
    {
        yield return www;
    }

    public static string LoadFile(string fileName, MonoBehaviour monoBehaviour)
    {
        if (fileName == "levelList.xml")
        {
            var pathPersistent = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            if (System.IO.File.Exists(pathPersistent))
            {
                return System.IO.File.ReadAllText(pathPersistent);
            }
        }
        var path = "jar:file://" + Application.dataPath + "!/assets/" + fileName;
        WWW www = new WWW(path);
        monoBehaviour.StartCoroutine(WaitForWWW(www));
        while (!www.isDone) { }
        return www.text;
    }

    public static void WriteToFile(string fileName, string content)
    {
        var path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        System.IO.File.WriteAllText(path, content);
    }
#endif

#if UNITY_STANDALONE_OSX
    public static void LoadFile(string fileName, MonoBehaviour monoBehaviour, Action<string> callback)
    {
        var path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        var content = System.IO.File.ReadAllText(path);
        callback(content);
    }


    public static void WriteToFile(string fileName, string content)
    {
        var path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        System.IO.File.WriteAllText(path, content);
    }
#endif

#if UNITY_WEBGL
    public static void LoadFile(string fileName, MonoBehaviour monoBehaviour, Action<string> callback)
    {
        monoBehaviour.StartCoroutine(LoadFileImpl(fileName, callback));
    }

    public static void WriteToFile(string fileName, string content)
    {
        var path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
        System.IO.File.WriteAllText(path, content);
    }

    static IEnumerator LoadFileImpl(string fileName, Action<string> callback)
    {
        string result;
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        Debug.Log("userDetailsXmlPath loading file:" + filePath);
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            WWW www = new WWW(filePath);
            yield return www;
            result = www.text;
        }
        else
            result = System.IO.File.ReadAllText(filePath);
        
        if (callback != null)
            callback(result);
    }
#endif

}
