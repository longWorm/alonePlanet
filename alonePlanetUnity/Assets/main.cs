using System.Collections;
using System.Threading;
using System.Collections.Generic;
using alonePlanetUnity.Assets;
using UnityEngine;

public class main : MonoBehaviour
{
    private GameObjectsManager _manager;
    public GameObject _planet;
    public GameObject _starPrefab;
    public GameObject _coinPrefab;
    public GameObject _planetCoordinatesText;
    public Animator _animator;
    public ParticleSystem _coinExplosion;

    private bool _inCollision = false;

#if UNITY_ANDROID

    IEnumerator WaitForWWW(WWW www)
    {
        yield return www;
    }

    public void LoadLevel()
    {
        var level = PlayerPrefs.GetString("level", "trulala");
        level += ".xml";
        Debug.Log(level);
        var path = "jar:file://" + Application.dataPath + "!/assets/" + level;
        WWW www = new WWW(path);
        StartCoroutine(WaitForWWW(www));
        while (!www.isDone) { }
        _manager = new GameObjectsManager(_starPrefab, _coinPrefab, www.text);
    }
#endif
#if UNITY_STANDALONE_OSX
    public void LoadLevel()
    {
        var level = PlayerPrefs.GetString("level", "trulala");
        Debug.Log(level);
        level += ".xml";
        var path = System.IO.Path.Combine(Application.streamingAssetsPath, level);
        var content = System.IO.File.ReadAllText(path);
        _manager = new GameObjectsManager(_starPrefab, _coinPrefab, content);
    }
#endif
    void Start()
    {
        LoadLevel();
        Respawn();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "star" && !_inCollision)
        {
            var ps = _planet.GetComponent<ParticleSystem>();
            ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
            emitOverride.startLifetime = 20f;
            ps.Emit(emitOverride, 100);
            _animator.SetTrigger("collision");
            _inCollision = true;
        }
        else if (col.gameObject.tag == "coin")
        {
            ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
            emitOverride.startLifetime = 20f;
            _coinExplosion.transform.position = col.gameObject.transform.position;
            _coinExplosion.Emit(emitOverride, 100);

            _manager.DestroyCoin(col.gameObject);
        }
    }

    public void Respawn()
    {
        _animator.ResetTrigger("collision");
        _inCollision = false;

        _planet.transform.position = _manager.PlanetInitialCoordinates;
        _planet.transform.localScale = _manager.PlanetInitialScale;
        _planet.GetComponent<ConstantForce>().force = new Vector3(0f, 0f, 0f);
        _manager.CreateCoins();
    }

    void Update()
    {
        UpdateForce();
    }

    private void UpdateForce()
    {
        var force = new Vector3(0, 0, 0);
        ProcessUserInput(ref force);
        ProcessStarsInteractions(ref force);
        _planet.GetComponent<ConstantForce>().force = new Vector3(force.x, force.y, 0.0f);
        UpdatePlanetCoordinatesText();
    }

    private void UpdatePlanetCoordinatesText()
    {
        var textMesh = _planetCoordinatesText.GetComponent<TextMesh>();
        textMesh.text = string.Format("x:{0:0.000} y:{1:0.000}", _planet.transform.position.x, _planet.transform.position.y);
    }

    private void ProcessUserInput(ref Vector3 force)
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            force.x -= GameObjectsManager.DeltaSpeedOnUserInput;
        else if (Input.GetKey(KeyCode.RightArrow))
            force.x += GameObjectsManager.DeltaSpeedOnUserInput;
        if (Input.GetKey(KeyCode.UpArrow))
            force.y += GameObjectsManager.DeltaSpeedOnUserInput;
        else if (Input.GetKey(KeyCode.DownArrow))
            force.y -= GameObjectsManager.DeltaSpeedOnUserInput;
        else if (Input.GetKey(KeyCode.R))
            Respawn();
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            var touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            force.x = (touchPos.x - _planet.transform.position.x) * GameObjectsManager.DeltaSpeedOnUserInput;
            force.y = (touchPos.y - _planet.transform.position.y) * GameObjectsManager.DeltaSpeedOnUserInput;
        }
#if UNITY_ANDROID
        else if (Input.touchCount > 0)
        {
            var touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            force.x = (touchPos.x - _planet.transform.position.x) * GameObjectsManager.DeltaSpeedOnUserInput * 0.1f;
            force.y = (touchPos.y - _planet.transform.position.y) * GameObjectsManager.DeltaSpeedOnUserInput * 0.1f;
        }
#endif
    }

    private void ProcessStarsInteractions(ref Vector3 force)
    {
        float deltaX = 0, deltaY = 0;
        foreach (var star in _manager._stars)
        {
            float distance = Vector3.Distance(_planet.transform.position, star.transform.position);
            float attractionForce = GameObjectsManager.Delta / Mathf.Pow(distance, 2);

            float xConstituent = star.transform.position.x - _planet.transform.position.x;
            float yConstituent = star.transform.position.y - _planet.transform.position.y;
            
            deltaX += (xConstituent / distance) * attractionForce;
            deltaY += (yConstituent / distance) * attractionForce;
        }

        force.x += deltaX; 
        force.y += deltaY;
    }
}
