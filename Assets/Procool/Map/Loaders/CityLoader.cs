using System;
using Procool.Rendering;
using UnityEngine;

namespace Procool.Map.Loader
{
    public class CityLoader : ContentLoader<City>
    {
        [SerializeField] private new CityRenderer renderer;
        public City City { get; private set; }

        protected override void Load(City city)
        {
            City = city;
            renderer.DrawCity(city);
        }

        public override void Unload()
        {
            renderer.Clear();
        }
    }
}