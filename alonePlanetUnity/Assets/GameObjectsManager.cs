using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace alonePlanetUnity.Assets
{
    public class GameObjectsManager : MonoBehaviour
    {
        public GameObject[] _stars;
        public GameObject[] _coins;

        public static float Delta = 5F;
        public static float DeltaSpeedOnUserInput = 1F;

        private Vector3 _planetInitialCoordinates;
        private Vector3 _planetInitialScale;

        protected struct Circle
        {
            public float x, y, r;
        }

        private Circle[] _coinsParameters;
        private GameObject _coinPrefab;
        private GameObject _starPrefab;

        public Vector3 PlanetInitialCoordinates
        {
            get { return _planetInitialCoordinates; }
            private set { _planetInitialCoordinates = value; }
        }
        public Vector3 PlanetInitialScale
        {
            get { return _planetInitialScale; }
            private set { _planetInitialScale = value; }
        }

        public GameObjectsManager(GameObject planet, GameObject starPrefab, GameObject coinPrefab, string text)
        {
            _coinPrefab = coinPrefab;
            _starPrefab = starPrefab;
            Debug.Log("gameobjectmanager");
            LoadLevel(text);
        }

        public void LoadLevel(string content)
        {
			XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(content);
			var planetPars = GetPlanet(ref xmldoc);
			_planetInitialCoordinates = new Vector3(planetPars.x, planetPars.y, 1f);
			_planetInitialScale = new Vector3(planetPars.r, planetPars.r, planetPars.r);
			
			var stars = GetStars(ref xmldoc);
			_stars = new GameObject[stars.GetLength(0)];
			var i = 0;
			foreach (var star in stars)
			{
				_stars[i] = CreateGO(_starPrefab, star);
				i++;
			}
			
			_coinsParameters = GetCoins(ref xmldoc);
			CreateCoins();            
		}

        public void CreateCoins()
        {
            if (_coins != null)
                foreach (var coin in _coins)
                    UnityEngine.Object.Destroy(coin);
            
			_coins = new GameObject[_coinsParameters.GetLength(0)];
			var i = 0;
			foreach (var coin in _coinsParameters)
			{
				_coins[i] = CreateGO(_coinPrefab, coin);
				i++;
			}
		}

        public void DestroyCoin(GameObject coin)
        {
            int index = 0;
            foreach (var coinIt in _coins)
            {
                if (coinIt == coin)
                {
                    var tmp = new List<GameObject>(_coins);
                    tmp.RemoveAt(index);
                    _coins = tmp.ToArray();
                    break;
                }
                index++;
            }
            
            UnityEngine.Object.Destroy(coin);
        }

        private static GameObject CreateGO(GameObject prefab, Circle par)
        {
            var result = GameObject.Instantiate(prefab, new Vector3(par.x, par.y, 5.0f), Quaternion.identity);
			result.transform.position = new Vector3(par.x, par.y, 1f);
			result.transform.localScale = new Vector3(par.r, par.r, par.r);
			result.SetActive(true);
            return result;
		}

        private static Circle GetCircleFromNode(XmlNode node)
        {
            Circle result = new Circle();
			result.x = float.Parse(node.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture);
			result.y = float.Parse(node.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture);
			result.r = float.Parse(node.Attributes.GetNamedItem("r").Value, CultureInfo.InvariantCulture);
            return result;
		}

        private static Circle[] GetStars(ref XmlDocument xmldoc)
        {
            Debug.Log("GetStars");
			XmlNodeList stars = xmldoc.SelectNodes("/Body/Stars/Star");
            Circle[] result = new Circle[stars.Count];
            int i = 0;
			foreach (XmlNode star in stars)
			{
                result[i] = GetCircleFromNode(star);
                i++;
			}
            return result;
		}

		private static Circle[] GetCoins(ref XmlDocument xmldoc)
		{
			XmlNodeList coins = xmldoc.SelectNodes("/Body/Coins/Coin");
			Circle[] result = new Circle[coins.Count];
			int i = 0;
			foreach (XmlNode coin in coins)
			{
				result[i] = GetCircleFromNode(coin);
				i++;
			}
			return result;
		}

        private static Circle GetPlanet(ref XmlDocument xmldoc)
        {
            XmlNode planet = xmldoc.SelectSingleNode("/Body/Planet");
            return GetCircleFromNode(planet);
        }
    }
}
