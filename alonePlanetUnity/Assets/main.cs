using System.Collections;
using System.Threading;
using System.Collections.Generic;
using alonePlanetUnity.Assets;
using UnityEngine;

public class main : MonoBehaviour {
    private GameObjectsManager _manager;
	public GameObject _planet;
	public GameObject _starPrefab;
    public GameObject _coinPrefab;
    public GameObject _planetCoordinatesText;
    public Animator _animator;

    void Start () 
    {
        _manager = new GameObjectsManager(_planet,_starPrefab,_coinPrefab);
        Respawn();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "star")
        {
            var ps = _planet.GetComponent<ParticleSystem>();
			ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
			emitOverride.startLifetime = 20f;
			ps.Emit(emitOverride, 20);

            //_animator.enabled = true;
            //WaitForAnimation();

            Respawn();
		}
        else if (col.gameObject.tag == "coin")
        {
            var ps = col.gameObject.GetComponent<ParticleSystem>();
            ps.transform.parent = null;
			ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
			emitOverride.startLifetime = 20f;
			ps.Emit(emitOverride, 100);

            //_manager.DestroyCoin(col.gameObject);
        }
    }

    private void Respawn()
    {
        _planet.transform.position = _manager.PlanetInitialCoordinates;
        _planet.transform.localScale = _manager.PlanetInitialScale;
        _planet.GetComponent<ConstantForce>().force = new Vector3(0f, 0f, 0f);
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
            Respawn();
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

    private void WaitForAnimation()
    {
        Debug.Log("WaitForAnimation enter");
  //      Thread.Sleep(10);
		//Debug.Log("WaitForAnimation enter2");

		while (true)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("absorbAnimationClip"))
                Debug.Log("absorbAnimationClip is running");
            else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("absorbAnimation"))
                Debug.Log("absorbAnimation is running");
            else
                break;
            //Thread.Sleep(10);
        }

        Debug.Log("Animation ended");
        //yield return new WaitUntil(() => _animator.isActiveAndEnabled == false);
	}
}
