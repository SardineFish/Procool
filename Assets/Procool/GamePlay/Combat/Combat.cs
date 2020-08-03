using UnityEngine;

namespace Procool.GamePlay.Combat
{
    public abstract class Combat : MonoBehaviour
    {

        public abstract void StartCombat();

        public abstract void StopCombat();
    }
}