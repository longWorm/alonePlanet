using System.Collections;
using System.Collections.Generic;
using alonePlanetUnity.Assets;
using UnityEngine;

public class main : MonoBehaviour {
    private GameObjectsManager _manager;
	public GameObject _planet;
	public GameObject _starPrefab;
    public GameObject _coinPrefab;
    public GameObject _planetCoordinatesText;

    void Start () {
        _manager = new GameObjectsManager(_planet,_starPrefab,_coinPrefab);
		_planet.transform.position = _manager.PlanetInitialPars;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "star")
        {
			var ps = _planet.GetComponent<ParticleSystem>();
			ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
			emitOverride.startLifetime = 20f;
			ps.Emit(emitOverride, 20);
			Invoke("Respawn", 0.5f);
		}
        else if (col.gameObject.tag == "coin")
        {
            var ps = col.gameObject.GetComponent<ParticleSystem>();
			ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
			emitOverride.startLifetime = 20f;
			ps.Emit(emitOverride, 20);

            StartCoroutine(DestroyCoin(col.gameObject, 0.5f));
        }
    }

    private IEnumerator DestroyCoin(GameObject coin, float delay)
    {
        yield return new WaitForSeconds(delay);
        _manager.DeleteCoin(coin);
    }

    private void Respawn()
    {
		_planet.transform.position = _manager.PlanetInitialPars;
    }

	void Update () {
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
			_planet.transform.position = _manager.PlanetInitialPars;
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
