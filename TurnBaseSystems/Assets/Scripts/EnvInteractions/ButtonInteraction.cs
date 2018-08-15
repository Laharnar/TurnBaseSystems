using UnityEngine;

/// <summary>
/// Activator for buttons over the player interactible content.
/// </summary>
public class ButtonInteraction : MonoBehaviour {

    public Interaction interaction;
    //public Structure source;
    public Weapon weaponSource;

    public bool destroyAfter = true;

    public void Activate() {
        interaction.Interact(weaponSource != null ? weaponSource as IInteractible : null);//source);
        if (destroyAfter)
            Destroy(gameObject);
    }
}