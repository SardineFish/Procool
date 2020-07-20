using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    [RequireComponent(typeof(HexagonGrid))]
    public class BlockGenerator : MonoBehaviour, ICustomEditorEX
    {
        public int Size = 4;
        
        [EditorButton]
        public void Generate()
        {
            
        }
    }
}