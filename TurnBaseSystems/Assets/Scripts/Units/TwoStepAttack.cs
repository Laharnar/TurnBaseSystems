public class TwoStepAttack:Interaction {
    public Unit unitSource;
    public int atkId;

    public TwoStepAttack Init(Unit unitSource, int atkId) {
        this.unitSource = unitSource;
        this.atkId = atkId;
        return this;
    }

    public override void Interact(IInteractible other) {
        PlayerFlag.m.SetActiveAbility(unitSource, atkId);
    }
}