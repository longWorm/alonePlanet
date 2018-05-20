using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace alonePlanetUnity.Assets
{
    public class GameObjectsManager
    {
        public GameObject[] _stars;
		public GameObject[] _walls;
        public GameObject[] _coins;
		public GameObject[] _arrows;

        private GameObject _canvasForControls, _canvasForGameObjects;

        public static float Delta = 10F;
        public static float DeltaSpeedOnUserInput = 1F;
        public static float DeltaSpeedOnUserInputTouch = 0.001F;

        private Vector3 _planetInitialCoordinates;
        private Vector3 _planetInitialScale;

        protected struct Circle
        {
            public float x, y, r;
        }

        protected struct Rectangle
        {
            public float x, y, w, h;
        }

        private Circle[] _coinsParameters, _starsParameters;
        private Rectangle[] _wallsParameters;

        private GameObject _coinPrefab, _wallPrefab, _starPrefab, _arrowPrefab;

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

        public GameObjectsManager(GameObject starPrefab, GameObject coinPrefab, GameObject wallPrefab, GameObject arrowPrefab
                                  , GameObject canvasForControls, GameObject canvasForGameObjects
                                  , string text)
        {
            _coinPrefab = coinPrefab;
            _wallPrefab = wallPrefab;
            _starPrefab = starPrefab;
            _arrowPrefab = arrowPrefab;
            _canvasForControls = canvasForControls;
            _canvasForGameObjects = canvasForGameObjects;

            LoadLevel(text);
            CreateGameObjects();
        }

        public void CreateGameObjects()
        {
            CreateCoins();
            CreateStars();
            CreateWalls();
            CreateArrows();
        }

        public void LoadLevel(string content)
        {
			XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(content);
			var planetPars = GetPlanet(ref xmldoc);
			_planetInitialCoordinates = new Vector3(planetPars.x, planetPars.y, 1f);
			_planetInitialScale = new Vector3(planetPars.r, planetPars.r, planetPars.r);
			
            _starsParameters = GetStars(ref xmldoc);
            _coinsParameters = GetCoins(ref xmldoc);
            _wallsParameters = GetWalls(ref xmldoc);
		}

        private void CreateArrows()
        {
            if (_arrows != null)
                foreach (var arrow in _arrows)
                    UnityEngine.Object.Destroy(arrow);

            _arrows = new GameObject[_coins.Length];
            for (int j = 0; j != _coins.Length; ++j)
                _arrows[j] = CreateGO(_arrowPrefab, _canvasForControls, 0.5f, 0.5f, 0);
        }

        private void CreateStars()
        {
            if (_stars != null)
                foreach (var star in _stars)
                    UnityEngine.Object.Destroy(star);

            _stars = new GameObject[_starsParameters.GetLength(0)];
            var i = 0;
            foreach (var star in _starsParameters)
            {
                _stars[i] = CreateGO(_starPrefab, _canvasForGameObjects, star);
                i++;
            }
        }

        private void CreateWalls()
        {
            if (_walls != null)
                foreach (var wall in _walls)
                    UnityEngine.Object.Destroy(wall);

            _walls = new GameObject[_wallsParameters.GetLength(0)];
            var i = 0;
            foreach (var wall in _wallsParameters)
            {
                _walls[i] = CreateGO(_wallPrefab, _canvasForGameObjects, wall);
                i++;
            }
        }

        private void CreateCoins()
        {
            if (_coins != null)
                foreach (var coin in _coins)
                    UnityEngine.Object.Destroy(coin);
            
			_coins = new GameObject[_coinsParameters.GetLength(0)];
			var i = 0;
			foreach (var coin in _coinsParameters)
			{
                _coins[i] = CreateGO(_coinPrefab, _canvasForGameObjects, coin);
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

        // Arrows
        private static GameObject CreateGO(GameObject prefab, GameObject parent, float x, float y, float angle)
        {
            var result = GameObject.Instantiate(prefab, new Vector3(x, y, 5.0f), Quaternion.identity);
            result.transform.position = new Vector3(x, y, 5.0f);
            result.SetActive(true);
            result.transform.SetParent(parent.transform, false);
            return result;
        }

        // Stars and coins
        private GameObject CreateGO(GameObject prefab, GameObject parent, Circle par)
        {
            var result = GameObject.Instantiate(prefab, new Vector3(par.x, par.y, 5.0f), Quaternion.identity);
			result.transform.position = new Vector3(par.x, par.y, 1f);
			result.transform.localScale = new Vector3(par.r, par.r, par.r);
            result.transform.SetParent(parent.transform);
			result.SetActive(true);
            return result;
		}

        // Walls
        private static GameObject CreateGO(GameObject prefab, GameObject parent, Rectangle par)
        {
            var result = GameObject.Instantiate(prefab, new Vector3(par.x, par.y, 5.0f), Quaternion.identity);
            result.transform.position = new Vector3(par.x, par.y, 1f);
            result.transform.localScale = new Vector3(par.w, par.h, 1f);
            result.transform.SetParent(parent.transform);
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

        private static Rectangle GetRectFromNode(XmlNode node)
        {
            Rectangle rect = new Rectangle();
            rect.x = float.Parse(node.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture);
            rect.y = float.Parse(node.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture);
            rect.w = float.Parse(node.Attributes.GetNamedItem("w").Value, CultureInfo.InvariantCulture);
            rect.h = float.Parse(node.Attributes.GetNamedItem("h").Value, CultureInfo.InvariantCulture);
            return rect;
        }

        private static Circle[] GetStars(ref XmlDocument xmldoc)
        {
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

        private static Rectangle[] GetWalls(ref XmlDocument xmldoc)
        {
            XmlNodeList walls = xmldoc.SelectNodes("/Body/Walls/Wall");
            Rectangle[] result = new Rectangle[walls.Count];
            int i = 0;
            foreach (XmlNode wall in walls)
            {
                result[i] = GetRectFromNode(wall);
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
