using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace alonePlanetUnity.Assets
{
    public class GameObjectsManager : MonoBehaviour
    {
        public GameObject _planet;
		public GameObject[] _stars;
		public GameObject[] _coins;
		
        public static float Delta = 5F;
        public static float DeltaSpeedOnUserInput = 1F;

        private Vector3 _planetInitialPars;    
        public Vector3 PlanetInitialPars{
            get { return _planetInitialPars; }
            private set { _planetInitialPars = value; }
        }

        public GameObjectsManager(GameObject planet, GameObject starPrefab, GameObject coinPrefab)
        {
            _planet = planet;
            var path = System.IO.Path.Combine(Application.streamingAssetsPath, "level1.xml");
            var content = System.IO.File.ReadAllText(path);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(content);

            var planetPars = GetPlanet(ref xmldoc);
            PlanetInitialPars = _planet.transform.position = new Vector3(planetPars.x, planetPars.y, 1f);
            _planet.transform.localScale = new Vector3(planetPars.r, planetPars.r, planetPars.r);

            var stars = GetStars(ref xmldoc);
            _stars = new GameObject[stars.GetLength(0)];
			var i = 0;
            foreach (var star in stars)
            {
                _stars[i] = CreateGO(starPrefab, star);
                i++;
            }

            var coins = GetCoins(ref xmldoc);
            _coins = new GameObject[coins.GetLength(0)];
			i = 0;
			foreach (var coin in coins)
			{
                _coins[i] = CreateGO(coinPrefab, coin);
				i++;
			}
		}

        public void DeleteCoin(GameObject coin)
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

			Destroy(coin);
        }

        protected struct Circle
        {
            public float x, y, r;
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
