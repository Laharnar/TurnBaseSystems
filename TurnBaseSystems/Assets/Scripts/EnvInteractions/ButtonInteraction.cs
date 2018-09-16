using UnityEngine;

/// <summary>
/// Activator for buttons over the player interactible content.
/// </summary>
public class ButtonInteraction : MonoBehaviour {

    public Interaction interaction;
    //public Structure source;
    public Weapon weaponSource;

    public bool destroyAfter = true;
    public static bool btnClicked = false;

    public void Activate() {
        //btnClicked = true;
        interaction.Interact();

        //PlayerUIAbilityList.m.showAbilityDescription = false;

        if (destroyAfter)
            Destroy(gameObject);
    }

    public void ActivateHelp() {
        //PlayerUIAbilityList.m.showAbilityDescription = true;
    }
}