using Unity.Mathematics;

namespace Utils
{
    public class DamageCalculatorUtil 
    {
        public static float CalculateDamage(Status targetStatus, Status attackerStatus, Attack attack)
        {
            float defence = math.clamp(targetStatus.Defense - attack.DefensePierce, Status.MIN_DEFENSE, Status.MAX_DEFENSE);
            float attackDamage = attack.DamageAmount + attackerStatus.Attack;
            float totalDamage = math.clamp(attackDamage - defence, Attack.MIN_DAMAGE, Attack.MAX_DAMAGE); 
        
            return totalDamage;
        }
    }
}
