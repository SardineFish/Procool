using System;
using Procool.GameSystems;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.Misc
{
    public class VehiclePaint : LazyLoadComponent
    {

        public override void Load()
        {
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                var arr = renderer.materials;
                arr[0] = ResourcesManager.Instance.VehiclePaints.RandomTake(GameRNG.GetScalar());
                renderer.materials = arr;
            }
        }

        public void SetBrokenPaint()
        {
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                var arr = renderer.materials;
                arr[0] = ResourcesManager.Instance.BrokenVehicle;
                renderer.materials = arr;
            }
        }
    }
}