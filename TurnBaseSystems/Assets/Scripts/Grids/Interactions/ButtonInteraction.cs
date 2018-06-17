using UnityEngine;
public class ButtonInteraction : MonoBehaviour {

    public Interaction interaction;
    public Structure source;
    public Weapon weaponSource;

    public void Activate() {
        interaction.Interact(weaponSource != null ? weaponSource as IInteractible : source);
        Destroy(gameObject);
    }
}