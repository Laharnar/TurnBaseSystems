using UnityEngine;
public class ButtonInteraction : MonoBehaviour {

    public Interaction interaction;
    public Structure source;

    public void Activate() {
        interaction.Interact(source);
    }
}