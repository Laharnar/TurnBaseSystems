using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CombatAbilityHandler1 {
    Queue<AbilityInfo> abilitiesQue = new Queue<AbilityInfo>();

    public void AddAbility(AbilityInfo info) {
        abilitiesQue.Enqueue(info);
    }

    /// <summary>
    /// Global ability handler... Executes single ability, animations and all.
    /// </summary>
    /// <returns></returns>
    public IEnumerator AbilityQueHandler() {

        while (true) {
            if (abilitiesQue.Count == 0) {
                yield return null;
                continue;
            }

            AbilityInfo info = abilitiesQue.Dequeue();
            // activates attack. these attacks can add further attacks to be executed.
            AbilityInfo.Instance = new AbilityInfo(info);
            info.executingUnit.AttackAction(info);

            if (info.executingUnit.dead) {
                CombatUI.OnUnitDeseleted();
            }
            // handles, data changes
            // move reaction is executed when attack is move.
            if (info.activeAbility == info.executingUnit.abilities.move2) {// move
                CombatEvents.ReapplyAuras(info.executingUnit.snapPos, info.attackedSlot, info.executingUnit);
            }

            // wait animations
            info.executingUnit.Animations_VFX_IfParsedAttack(info.activeAbility);


            float len = AttackData2.AnimLength(info.executingUnit, info.activeAbility);
            if (len > 0)
                yield return new WaitForSeconds(len);


            // movement

            // effects

            // sound

            // reset
            //AbilityInfo.Instance.Reset(); don't reset combat after 1 atk.
        }
    }

}
