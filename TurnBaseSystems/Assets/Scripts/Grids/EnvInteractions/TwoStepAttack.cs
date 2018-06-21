public class TwoStepAttack:Interaction {
    public UnitAbilities unitSource;
    public int atkId;

    public TwoStepAttack Init(UnitAbilities unitSource, int atkId) {
        this.unitSource = unitSource;
        this.atkId = atkId;
        return this;
    }

    public override void Interact(IInteractible other) {
        PlayerFlag.m.StartTwoStepAttack(unitSource, atkId);
    }
}