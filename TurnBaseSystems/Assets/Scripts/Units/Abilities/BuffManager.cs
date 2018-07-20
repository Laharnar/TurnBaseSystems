using System.Collections.Generic;
public class BuffManager {

    public static List<BUFFAttackData> registeredBuffs = new List<BUFFAttackData>();
    public static List<Unit> registeredBuffSources = new List<Unit>();

    public static void Register(Unit source, BUFFAttackData attackDataType) {
        registeredBuffSources.Add(source);
        registeredBuffs.Add(attackDataType.Copy());
    }

    public static void ConsumeBuffs(int faction) {
        for (int i = 0; i < registeredBuffs.Count; i++) {
            if (registeredBuffSources[i] == null) {
                registeredBuffs.RemoveAt(i);
                registeredBuffSources.RemoveAt(i);
                continue;
            }

            // type: consume only on enemy faction.
            if (registeredBuffSources[i].flag.allianceId != faction) {
                registeredBuffs[i].turns--;

                OnTurnConsumed(i);
            }
        }
    }

    static void OnTurnConsumed(int buffIndex) {
        int i = buffIndex;
        if (registeredBuffs[i].turns <= 0) {
            AttackData2.RunAnimations(registeredBuffSources[i], registeredBuffs[i].endAnimSets);
            registeredBuffSources[i].combatStatus = registeredBuffs[i].endBuffStatus;

            registeredBuffs.RemoveAt(i);
            registeredBuffSources.RemoveAt(i);
        }
    }
}