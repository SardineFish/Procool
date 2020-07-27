using System.Collections.Generic;
using System.Linq;
using Procool.GameSystems;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class WeaponConstructor
    {
        public struct PossibleBehaviour
        {
            public BehaviourConstructData ConstructData;
            public float Probability;
            public IWeaponBehaviour Behaviour => ConstructData.Behaviour;

            public PossibleBehaviour(BehaviourConstructData constructData, float probability)
            {
                ConstructData = constructData;
                Probability = probability;
            }
        }

        public class BehaviourConstructData
        {
            public IWeaponBehaviour Behaviour;
            public bool UseInFirstStage = false;
            public float Probability = 1;
            public int StageDepthLimit = -1;
            public List<PossibleBehaviour> CompatibleParallels = new List<PossibleBehaviour>();
            public List<PossibleBehaviour> NextStages = new List<PossibleBehaviour>();
            public WeaponConstructor Constructor;

            public BehaviourConstructData(IWeaponBehaviour behaviour, WeaponConstructor constructor)
            {
                Behaviour = behaviour;
                Constructor = constructor;
            }
        }

        public class BehaviourConstructor<T> : BehaviourConstructData where T : IWeaponBehaviour
        {
            public BehaviourConstructor<T> CompatibleWith<TBehaviour>(float probability = 1)
                where TBehaviour : IWeaponBehaviour, new()
            {
                CompatibleParallels.Add(new PossibleBehaviour(Constructor.GetBehaviourConstructor<TBehaviour>(),
                    probability));
                return this;
            }

            public BehaviourConstructor<T> NextStage<TBehaviour>(float probability = 1)
                where TBehaviour : IWeaponBehaviour, new()
            {
                NextStages.Add(new PossibleBehaviour(Constructor.GetBehaviourConstructor<TBehaviour>(), probability));
                return this;
            }

            public BehaviourConstructor<T> CanBeFirstStage()
            {
                UseInFirstStage = true;
                return this;
            }

            /// <summary>
            /// The probability weight to have no next stage.
            /// </summary>
            /// <param name="probability"></param>
            /// <returns></returns>
            public BehaviourConstructor<T> Terminate(float probability)
            {
                NextStages.Add(new PossibleBehaviour(null, probability));
                return this;
            }

            public BehaviourConstructor<T> DepthLimit(int depth)
            {
                StageDepthLimit = depth;
                return this;
            }

            public BehaviourConstructor(IWeaponBehaviour behaviour, WeaponConstructor constructor) : base(behaviour,
                constructor)
            {
            }
        }

        public List<PossibleBehaviour> Behaviours = new List<PossibleBehaviour>();

        Dictionary<System.Type, BehaviourConstructData> ConstructDatas =
            new Dictionary<System.Type, BehaviourConstructData>();

        Dictionary<IWeaponBehaviour, BehaviourConstructData> ConstructDataByBehaviour =
            new Dictionary<IWeaponBehaviour, BehaviourConstructData>();

        public BehaviourConstructor<T> Behaviour<T>(float probability = 1)
            where T : IWeaponBehaviour, new()
        {
            var constructor = GetBehaviourConstructor<T>();
            Behaviours.Add(new PossibleBehaviour(constructor, probability));
            return constructor;
        }

        BehaviourConstructor<T> GetBehaviourConstructor<T>() where T : IWeaponBehaviour, new()
        {
            var type = typeof(T);
            if (!ConstructDatas.ContainsKey(type))
            {
                var behaviour = new T();
                var constructData = new BehaviourConstructor<T>(behaviour, this);
                ConstructDatas[type] = constructData;
                ConstructDataByBehaviour[behaviour] = constructData;
                return constructData;
            }

            return ConstructDatas[type] as BehaviourConstructor<T>;
        }

        public DamageStage BuildStage(PRNG prng, IEnumerable<PossibleBehaviour> behaviours, bool firstStage, int depth,
            int depthLimit)
        {
            if (depth >= depthLimit)
                return null;

            var maxComponents = prng.GetInRange(1, 4);
            var possibleBehaviours = ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Get();
            possibleBehaviours.Clear();

            behaviours
                .Where(b => (!firstStage) || (firstStage && b.ConstructData.UseInFirstStage))
                .ForEach(behaviour => possibleBehaviours.Add(behaviour.Behaviour, behaviour));

            var stage = new DamageStage();
            for (var i = 0; i < maxComponents; i++)
            {
                var behaviour = possibleBehaviours.Values.RandomTake(prng.GetScalar(), b => b.Probability);

                if (behaviour.ConstructData is null)
                {
                    if (i == 0)
                    {
                        ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Release(possibleBehaviours);
                        return null;
                    }

                    continue;
                }

                var behaviourData = behaviour.Behaviour.GenerateBehaviourData(prng);
                stage.Behaviours.Add(behaviourData);

                var intersectSet = ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Get();
                intersectSet.Clear();
                foreach (var compatibleBehaviour in behaviour.ConstructData.CompatibleParallels)
                {
                    if (possibleBehaviours.ContainsKey(compatibleBehaviour.Behaviour))
                    {
                        var currentPossible = possibleBehaviours[compatibleBehaviour.Behaviour];
                        intersectSet.Add(compatibleBehaviour.Behaviour,
                            new PossibleBehaviour(compatibleBehaviour.ConstructData,
                                currentPossible.Probability + compatibleBehaviour.Probability));
                    }
                }

                ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Release(possibleBehaviours);
                possibleBehaviours = intersectSet;

                var nextDepthLimit = behaviour.ConstructData.StageDepthLimit;
                if (nextDepthLimit < 0)
                    nextDepthLimit = depthLimit;
                behaviourData.NextStage = BuildStage(prng, behaviour.ConstructData.NextStages, false, depth + 1,
                    nextDepthLimit);
            }

            ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Release(possibleBehaviours);
            return stage;
        }

        public Weapon BuildWeapon(PRNG prng)
        {
            var weapon = new Weapon();

            var depthLimit = prng.GetInRange(3, 5);
            var stage = BuildStage(prng, Behaviours, true, 0, depthLimit);

            weapon.FirstStage = stage;
            return weapon;
        }
    }
}