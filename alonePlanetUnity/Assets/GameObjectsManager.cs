using System;
using System.Xml;
using System.IO;
using UnityEngine;
using System.Globalization;

namespace alonePlanetUnity.Assets
{
    public class GameObjectsManager
    {
        public GameObject _planet;
        public GameObject[] _stars;
		
        public static float Delta = 5F;
        public static float DeltaSpeedOnUserInput = 1F;
            
        public Vector2 planetSpeed = new Vector2(0, 0);
        public Vector2 _force = new Vector2(0, 0);

		public Vector3 PlanetInitialPars;

        public GameObjectsManager(GameObject planet, GameObject starPrefab)
        {
            _planet = planet;
            var path = System.IO.Path.Combine(Application.streamingAssetsPath, "level1.xml");
            var content = System.IO.File.ReadAllText(path);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(content);

            var stars = GetStars(ref xmldoc);
            var planetPars = GetPlanet(ref xmldoc);
            PlanetInitialPars = _planet.transform.position = new Vector3(planetPars.x, planetPars.y, 1f);
            _planet.transform.localScale = new Vector3(planetPars.r, planetPars.r, planetPars.r);
            var i = 0;
            _stars = new GameObject[stars.GetLength(0)];
            foreach (var star in stars)
            {
                _stars[i] = GameObject.Instantiate(starPrefab, new Vector3(star.x, star.y, 5.0f), Quaternion.identity);
                _stars[i].transform.position = new Vector3(star.x, star.y, 1f);
                _stars[i].transform.localScale = new Vector3(star.r, star.r, star.r);
                _stars[i].SetActive(true);
                i++;
            }
        }

        protected struct Circle
        {
            public float x, y, r;
        }


        private Circle GetCircleFromNode(XmlNode node)
        {
            Circle result = new Circle();
			result.x = float.Parse(node.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture);
			result.y = float.Parse(node.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture);
			result.r = float.Parse(node.Attributes.GetNamedItem("r").Value, CultureInfo.InvariantCulture);
            return result;
		}

        private Circle[] GetStars(ref XmlDocument xmldoc)
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

        private Circle GetPlanet(ref XmlDocument xmldoc)
        {
            XmlNode planet = xmldoc.SelectSingleNode("/Body/Planet");
            return GetCircleFromNode(planet);
        }

        public void LoadLevel()
        {
            
        }
    }
}
