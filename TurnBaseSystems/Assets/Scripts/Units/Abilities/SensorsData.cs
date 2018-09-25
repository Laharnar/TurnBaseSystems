using System;
/// <summary>
/// What happens when enemy or ally enters range.
/// </summary>
[System.Serializable]
public class SensorsData : AbilityEffect {
    public GridMask auraRange;
    public bool explode;

    public TargetFilter targetFilter;
    public SensorTrigger activationTrigger;

    internal override void AtkBehaviourExecute(AbilityInfo info) {
        switch (activationTrigger) {
            case SensorTrigger.OnUnitEnters:
                if (info.activator.onMove) {
                    // current system doesn't handle on attack response.
                }
                break;
            case SensorTrigger.OnUnitExits:
                // current system doesn't handle on attack response.
                break;
            case SensorTrigger.OnUnitEnterExists:
                // current system doesn't handle on attack response.
                break;
            case SensorTrigger.OnAnyTurnEnd:
                //
                break;
            case SensorTrigger.OnAllyTurnEnd:
                //
                break;
            case SensorTrigger.OnEnemyTurnEnd:
                //
                break;
            case SensorTrigger.OnAttacked:
                // this should be triggered when the attack happens, by other units.
                // current system doesn't handle on attack response.
                /*
                if (info.activator.onAttack) {
                    GridAccess.GetUnitAtPos(info.attackedSlot);
                    info.executingUnit = 
                    Execute();
                }*/
                break;
            case SensorTrigger.OnIsSteppedOn:
                // in this case, the executing unit is this unit with ability,
                // and the check tells if someone else is standing on it.
                Combat c = GameManager.Instance.GetManager<Combat>();
                if ((info.activator.onMove && Unit.IsUnderSomeone(info.executingUnit, c.flagsManager.GetVisibleUnits(c.activeFlag, info.executingUnit).ToArray()))) {
                    Execute();
                }
                break;
            default:
                break;
        }
    }

    private void Execute() {
        if (explode) {
            //AbilityInfo.ActiveAbility.aoe.damage
        }
    }
}
