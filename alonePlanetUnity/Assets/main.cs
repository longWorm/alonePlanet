using System.Collections;
using System.Collections.Generic;
using alonePlanetUnity.Assets;
using UnityEngine;

public class main : MonoBehaviour {
    private GameObjectsManager _manager;
	public GameObject _planet;
    public GameObject _starPrefab;
	
    void Start () {
        _manager = new GameObjectsManager(_planet,_starPrefab);
		_planet.transform.position = _manager.PlanetInitialPars;

    }

    void OnCollisionEnter(Collision col)
    {
		var ps = _planet.GetComponent<ParticleSystem>();
        ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
		emitOverride.startLifetime = 20f;
		ps.Emit(emitOverride, 20);

        Invoke("Respawn", 0.5f);
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
        //Debug.Log("forceX=" + deltaX + " forceY=" + deltaY);
        force.x += deltaX; 
        force.y += deltaY;
    }
}
