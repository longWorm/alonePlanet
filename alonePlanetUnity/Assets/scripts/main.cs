using System;
using System.Collections.Generic;
using System.Xml;
using alonePlanetUnity.Assets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    private GameObjectsManager _manager;
    public GameObject _planet;
    public GameObject _starPrefab, _wallPrefab, _coinPrefab, _arrowPrefab, _teleportPrefab, _explosionPrefab;
    public GameObject _planetCoordinatesText;
    public GameObject _camera;
    public GameObject _canvasForControls, _canvasForGameObjects;

    private GameObject _explosion;
    private bool[] _arrowsVisible;

    public Animator _animator;
    public ParticleSystem _coinExplosion;

    private bool _inCollision = false, _levelFinished=false, _teleporting=false;
    private int _teleportIndex = 0;

    private GameObject Explosion
    {
        get {
            if (!_explosion)
                _explosion = GameObject.Instantiate(_explosionPrefab, _planet.transform.position, Quaternion.identity);
            return _explosion;
        }
    }

    void Start()
    {
        FileReader.LoadFile(GetCurrentLevel(), this, this.LevelIsLoaded);
        Lean.Touch.LeanTouch.OnGesture += OnGesture;

        for (int i = 0; i != _canvasForControls.transform.childCount; ++i)
        {
            var child = _canvasForControls.transform.GetChild(i);
            if (child.tag == "zoomEnlarge")
                child.GetComponent<Button>().onClick.AddListener(delegate { Zoom(0.8f); });
            else if (child.tag == "zoomReduce")
                child.GetComponent<Button>().onClick.AddListener(delegate { Zoom(1.2f); });
            else if (child.tag == "nextLevelButton")
                child.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("mainScene"); });
            else if (child.tag == "mainMenuButton")
                child.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene("SelectLevel"); });
        }
    }

    public void LevelIsLoaded(string content)
    {
        _manager = new GameObjectsManager(_starPrefab, _coinPrefab, _wallPrefab, _arrowPrefab, _teleportPrefab
                                          , _canvasForControls, _canvasForGameObjects, content);
        _arrowsVisible = new bool[_manager._arrows.Length];
        for (int i = 0; i != _arrowsVisible.Length; ++i)
            _arrowsVisible[i] = false;
        Respawn();        
    }

    private void OnDisable()
    {
        Lean.Touch.LeanTouch.OnGesture -= OnGesture;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "teleport" && !_teleporting)
        {
            Debug.Log("teleport");
            _teleporting = true;
            _teleportIndex = col.gameObject.GetComponent<TeleportGameObject>()._index;
            _animator.SetTrigger("grow");
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "star" && !_inCollision)
        {
            var ps = _planet.GetComponent<ParticleSystem>();
            ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams
            {
                startLifetime = 20f
            };
            ps.Emit(emitOverride, 100);
            _animator.SetTrigger("collision");
            _inCollision = true;
            Explosion.transform.position = _planet.transform.position;
            Explosion.GetComponent<ParticleSystem>().Play();
        }
        else if (col.gameObject.tag == "coin")
        {
            ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams
            {
                startLifetime = 20f
            };
            _coinExplosion.transform.position = col.gameObject.transform.position;
            _coinExplosion.Emit(emitOverride, 100);

            _manager.DestroyCoin(col.gameObject);
            if (_manager._coins.Length == 0)
                OnLevelComplete();
        }
    }

    public void Teleport()
    {
        int bindedTeleportIndex = 0;
        switch (_teleportIndex % 2)
        {
            case 0:
                bindedTeleportIndex = _teleportIndex + 1;
                break;
            case 1:
                bindedTeleportIndex = _teleportIndex - 1;
                break;
        }
        _planet.transform.position = _manager._teleports[bindedTeleportIndex].transform.position;
    }

    public void ResetTeleportingFlag()
    {
        _teleporting = false;
    }

    public void Respawn()
    {
        _animator.ResetTrigger("collision");
        _inCollision = _teleporting = false;

        _planet.SetActive(false);
        _planet.transform.position = _manager.PlanetInitialCoordinates;
        _planet.transform.localScale = _manager.PlanetInitialScale;
        _planet.transform.SetParent(_canvasForGameObjects.transform);
        _planet.SetActive(true);
        _planet.GetComponent<ConstantForce>().force = new Vector3(0f, 0f, 0f);
        _planet.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        _planet.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);

        _manager.CreateGameObjects();
    }

    void Update()
    {
        if (_manager == null || _levelFinished)
            return;
        UpdateForce();
        _camera.transform.position = new Vector3(_planet.transform.position.x, _planet.transform.position.y, -10f);
        UpdateArrowPosition();
    }

    private void UpdateForce()
    {
        var force = new Vector3(0, 0, 0);
        ProcessUserInput(ref force);
        ProcessStarsInteractions(ref force);
        _planet.GetComponent<ConstantForce>().force = new Vector3(force.x, force.y, 0.0f);
        //UpdatePlanetCoordinatesText();
    }

    private void UpdateArrowPosition()
    {
        if (_manager._coins.Length == 0)
            return;

        int index = 0;
        foreach(var coin in _manager._coins)
        {
            if (coin.GetComponent<Renderer>().isVisible)
            {
                if (_arrowsVisible[index])
                {
                    _arrowsVisible[index] = false;
                    _manager._arrows[index].SetActive(false);
                }
            }
            else
            {
                if (!_arrowsVisible[index])
                {
                    _arrowsVisible[index] = true;
                    _manager._arrows[index].SetActive(true);
                }

                var xDistance = _planet.transform.position.x - coin.transform.position.x;
                var yDistance = _planet.transform.position.y - coin.transform.position.y;

                var angle = Math.Atan(yDistance / xDistance);

                var x = (float)Math.Cos(angle) * 16f;
                var y = (float)Math.Sin(angle) * 16f;

                if (xDistance > 0)
                {
                    x *= -1;
                    y *= -1;
                    angle += Math.PI;
                }

                _manager._arrows[index].transform.localPosition = new Vector3(x, y, 4f);
                var targetRotation = Quaternion.Euler(0, 0, (float)(angle * 180 / Math.PI - 90));
                _manager._arrows[index].transform.rotation = Quaternion.Slerp(_manager._arrows[index].transform.rotation, targetRotation, 5f);
            }
            ++index;
        }
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
            force.x += (touchPos.x - _planet.transform.position.x) * GameObjectsManager.DeltaSpeedOnUserInput;
            force.y += (touchPos.y - _planet.transform.position.y) * GameObjectsManager.DeltaSpeedOnUserInput;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            Zoom(1.2f);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            Zoom(0.83f);
#if UNITY_ANDROID
        else if (Input.touchCount == 1)
        {
            var touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            bool changeX = Math.Abs(touchPos.x - _planet.transform.position.x) > 0.1f;
            bool changeY = Math.Abs(touchPos.y - _planet.transform.position.y) > 0.1f;

            if (changeX)
                force.x += (touchPos.x > _planet.transform.position.x) ? GameObjectsManager.DeltaSpeedOnUserInputTouch : (-1) * GameObjectsManager.DeltaSpeedOnUserInputTouch;

            if (changeY)
                force.y += (touchPos.y > _planet.transform.position.y) ? GameObjectsManager.DeltaSpeedOnUserInputTouch : (-1) * GameObjectsManager.DeltaSpeedOnUserInputTouch;
        }
#endif
    }

    private void OnGesture(List<Lean.Touch.LeanFinger> fingers)
    {
        Zoom(1 / Lean.Touch.LeanGesture.GetPinchScale(fingers));
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

    private string GetCurrentLevel()
    {
        var level = PlayerPrefs.GetString(GameConstants.CurrentLevel, "1"/*default*/);
        level += ".xml";
        return level;
    }

    private void OnLevelComplete()
    {
        MarkCurrentLevelAsCompleted();

        _planet.GetComponent<ConstantForce>().force = new Vector3(0f, 0f, 0f);
        _planet.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        _planet.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
        _levelFinished = true;

        for (int i = 0; i != _canvasForControls.transform.childCount; ++i)
        {
            var child = _canvasForControls.transform.GetChild(i);
            if (child.tag == "nextLevelButton" || child.tag == "mainMenuButton" || child.tag == "levelCompleteText")
                child.gameObject.SetActive(true);
        }
    }

    private void Zoom(float scale)
    {
        var oldValue = _camera.GetComponent<Camera>().orthographicSize;
        var newValue = oldValue * scale;
        if (newValue < GameConstants.MinimumZoom)
            newValue = GameConstants.MinimumZoom;
        else if (newValue > GameConstants.MaximiumZoom)
            newValue = GameConstants.MaximiumZoom;

        if (newValue != oldValue)
            _camera.GetComponent<Camera>().orthographicSize = newValue;
    }

    private void MarkCurrentLevelAsCompleted()
    {
        if (PlayerPrefs.GetString(GameConstants.CurrentLevel).Length == 0)
            return;

        FileReader.LoadFile("levelList.xml", this, MarkCurrentLevelAsCompletedCallback);
    }

    public void MarkCurrentLevelAsCompletedCallback(string content)
    {
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(content);
        var currentLevel = Convert.ToInt16(PlayerPrefs.GetString(GameConstants.CurrentLevel));
        currentLevel++;
        var node = xmldoc.SelectSingleNode("/levels/level[@file=\"" + Convert.ToString(currentLevel) + "\"]");
        if (node == null)
            return;

        node.Attributes["enabled"].Value = "true";

        var root = xmldoc.SelectSingleNode("/levels");
        if (root.Attributes["currentLevel"] == null)
        {
            XmlAttribute newAttribute = xmldoc.CreateAttribute("currentLevel");
            newAttribute.Value = Convert.ToString(currentLevel);
            root.Attributes.Append(newAttribute);
        }
        else
            root.Attributes["currentLevel"].Value = Convert.ToString(currentLevel);

        FileReader.WriteToFile("levelList.xml", xmldoc.OuterXml);
        PlayerPrefs.SetString(GameConstants.CurrentLevel, Convert.ToString(currentLevel));        
    }
}
