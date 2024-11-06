namespace GameOff2024.Health
{
    public interface IDamagable
    {
        public bool TakeDamage(int amount);
        public bool IsDead();
    }
}
