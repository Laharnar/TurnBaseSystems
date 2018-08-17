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
        Unit selectedUnit = CombatData.Instance.selectedUnit;//selectedPlayerUnit;
        if (!selectedUnit)
            selectedText.text = "SELECT UNIT";
        else {
            bool selectedIsPlayer = selectedUnit.flag.allianceId == 0;
            if (selectedIsPlayer) {
                selectedText.text = selectedUnit.codename +
                    "\nHP | " + selectedUnit.hp + "/" + selectedUnit.maxHp +
                    "\nAP | " + selectedUnit.ActionsLeft + "/" + selectedUnit.maxActions +
                    (selectedUnit.temporaryArmor > 0 ? "\nArmor | " + selectedUnit.temporaryArmor : "") +
                    (selectedUnit.charges > 0 ? "\nCharges | " + selectedUnit.charges + "/" + selectedUnit.maxCharges : "");
            } else {// enemy
                selectedText.text = selectedUnit.codename +
                    "\nHP | " + selectedUnit.hp + "/" + selectedUnit.maxHp +
                    (selectedUnit.temporaryArmor > 0 ? "\nArmor | " + selectedUnit.temporaryArmor : "");
            }
        }

        Unit hoveredUnit = CombatData.Instance.hoveredUnit;
        if (hoveredUnit) {
            bool hoveredIsPlayer = hoveredUnit.flag.allianceId == 0;
            if (hoveredIsPlayer && hoveredUnit != CombatData.Instance.selectedPlayerUnit) {
                hoverText.text = hoveredUnit.codename+
                    "\nHP | " + hoveredUnit.hp + "/" + hoveredUnit.maxHp+
                    "\nAP | " + hoveredUnit.ActionsLeft + "/" + hoveredUnit.maxActions +
                    (hoveredUnit.temporaryArmor > 0 ? "\nArmor | " + hoveredUnit.temporaryArmor : "") +
                    (hoveredUnit.charges > 0 ? "\nCharges | " + hoveredUnit.charges + "/" + hoveredUnit.maxCharges : "");

            } else if (!hoveredIsPlayer && hoveredUnit != CombatData.Instance.selectedPlayerUnit) {// enemy
                hoverText.text = hoveredUnit.codename + 
                    "\nHP | " + hoveredUnit.hp + "/" + hoveredUnit.maxHp+
                    (hoveredUnit.temporaryArmor > 0 ? "\nArmor | " + hoveredUnit.temporaryArmor : "");
            }
        } else {
            hoverText.text = "";
        }
    }
}
