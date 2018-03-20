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
        var path = "jar:file://" + Application.dataPath + "!/assets/" + fileName;
        WWW www = new WWW(path);
        monoBehaviour.StartCoroutine(WaitForWWW(www));
        while (!www.isDone) { }
        return www.text;
    }
#endif

#if UNITY_STANDALONE_OSX
    public static string LoadFile(string fileName, MonoBehaviour monoBehaviour)
    {
        var path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        var content = System.IO.File.ReadAllText(path);
        return content;
    }
#endif
}
