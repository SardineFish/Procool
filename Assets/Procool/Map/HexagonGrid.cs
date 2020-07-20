using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Procool.Utils;

namespace Procool.Map
{
    public class HexagonGrid : ManagedMonobehaviour<HexagonGrid>
    {
        public bool showGrid = true;
        public float size = 1;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnDrawGizmosSelected()
        {
        }
    }

}
