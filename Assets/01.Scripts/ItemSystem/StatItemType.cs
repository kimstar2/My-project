namespace _01.Scripts.ItemSystem
{
    public enum StatItemType
    {
        MaxHealth,
        Damage,
        Speed,
        AttackSpeed
    }

    public static class StatItemTypeVar
    {
        public const string MaxHealth = "최대 체력";
        public const string Damage = "데미지";
        public const string Speed = "이동 속도";
        public const string AttackSpeed = "공격 속도";
    }
}