using Procool.GamePlay.Combat;
using Procool.GamePlay.Controller;
using Procool.Map;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class CombatSystem : Singleton<CombatSystem>
    {
        public Vector2 StreetFightRange = new Vector2(80, 200);
        
        public StreetFight GenerateStreetFight(City city, Vector2 location)
        {
            var size = Mathf.Lerp(StreetFightRange.x, StreetFightRange.y, GameRNG.GetScalarByVec2(location));
            var streetFight = GameObjectPool.Get<StreetFight>();
            streetFight.PreLoadCombat(city, location, size);
            return streetFight;
        }
    }
}