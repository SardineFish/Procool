namespace Procool.GamePlay.Mission
{
    public enum RewardType
    {
        None,
        Money,
        Weapon,
    }
    public struct MissionReward
    {
        public RewardType Type;
        public int Value;

        public override string ToString()
        {
            switch (Type)
            {
                case RewardType.Money:
                    return $"Money ${Value}";
                case RewardType.Weapon:
                    return $"Weapon x1";
                default:
                    return "Nothing";
            }
        }
    }
}