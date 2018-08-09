using System.Collections.Generic;
using UnityEngine;
public class AttackDataLib:MonoBehaviour {
    public AttackRangeData[] ranges;
    public StandardAttackData[] standards;
    public AOEAttackData[] aoes;
    public BUFFAttackData[] buffs;
    public EmpowerAlliesData[] auras;
    public MoveAttackData[] moves;
    public PassiveData[] passives;
    public PierceAtkData[] pierces;
    public SpawnAttackData[] spawns;

    internal AbilityEffect[] GetLib() {
        List<AbilityEffect> l = new List<AbilityEffect>();
        l.AddRange(ranges);
        l.AddRange(standards);
        l.AddRange(aoes);
        l.AddRange(buffs);
        l.AddRange(auras);
        l.AddRange(moves);
        l.AddRange(passives);
        l.AddRange(pierces);
        l.AddRange(spawns);
        return l.ToArray();
    }
}
