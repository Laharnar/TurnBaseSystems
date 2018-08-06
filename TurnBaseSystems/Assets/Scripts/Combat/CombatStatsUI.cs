using UnityEngine;
/// <summary>
/// Combat player UI. Display stats for selected units.
/// </summary>
public class CombatStatsUI :MonoBehaviour{

    public UnityEngine.UI.Text selectedText;
    public UnityEngine.UI.Text hoverText;

    private void Update() {
        UpdateStats();
    }

    public void UpdateStats() {
        if (PlayerFlag.selectedPlayerUnit) {
            selectedText.text = PlayerFlag.selectedUnit.codename + 
                "\nHP | " + PlayerFlag.selectedUnit.hp +"/"+PlayerFlag.selectedUnit.maxHp+
                "\nAP | " + PlayerFlag.selectedUnit.ActionsLeft + "/" + PlayerFlag.selectedUnit.maxActions +
                (PlayerFlag.selectedUnit.temporaryArmor > 0 ? "\nArmor | " + PlayerFlag.selectedUnit.temporaryArmor : "")+
                (PlayerFlag.selectedUnit.charges > 0 ? "\nCharges | " + PlayerFlag.selectedUnit.charges + "/"+PlayerFlag.selectedUnit.maxCharges: "");

        } else if (PlayerFlag.selectedUnit) {// enemy
            selectedText.text = PlayerFlag.selectedUnit.codename +
                "\nHP | " + PlayerFlag.selectedUnit.hp + "/" + PlayerFlag.selectedUnit.maxHp+
                (PlayerFlag.selectedUnit.temporaryArmor > 0 ? "\nArmor | " + PlayerFlag.selectedUnit.temporaryArmor : "");
        } else {
            selectedText.text = "SELECT UNIT";
        }

        if (PlayerFlag.hoveredUnit) {
            if (PlayerFlag.hoveredUnit.flag.allianceId == 0 && PlayerFlag.hoveredUnit != PlayerFlag.selectedPlayerUnit) {
                hoverText.text = PlayerFlag.hoveredUnit.codename+
                    "\nHP | " + PlayerFlag.hoveredUnit.hp + "/" + PlayerFlag.hoveredUnit.maxHp+
                    "\nAP | " + PlayerFlag.hoveredUnit.ActionsLeft + "/" + PlayerFlag.hoveredUnit.maxActions +
                    (PlayerFlag.hoveredUnit.temporaryArmor > 0 ? "\nArmor | " + PlayerFlag.hoveredUnit.temporaryArmor : "") +
                    (PlayerFlag.hoveredUnit.charges > 0 ? "\nCharges | " + PlayerFlag.hoveredUnit.charges + "/" + PlayerFlag.hoveredUnit.maxCharges : "");

            } else if (PlayerFlag.hoveredUnit.flag.allianceId != 0 && PlayerFlag.hoveredUnit != PlayerFlag.selectedPlayerUnit) {// enemy
                hoverText.text = PlayerFlag.hoveredUnit.codename + 
                    "\nHP | " + PlayerFlag.hoveredUnit.hp + "/" + PlayerFlag.hoveredUnit.maxHp+
                    (PlayerFlag.hoveredUnit.temporaryArmor > 0 ? "\nArmor | " + PlayerFlag.hoveredUnit.temporaryArmor : "");
            }
        } else {
            hoverText.text = "";
        }
    }
}
