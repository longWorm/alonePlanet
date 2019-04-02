using UnityEngine;
using System.Collections;

public class TeleportGameObject : MonoBehaviour
{
    public int _index = 0;
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("teleport");
    }
}
