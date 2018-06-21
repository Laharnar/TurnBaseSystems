using System.Collections;

public abstract class AiLogic : UnityEngine.MonoBehaviour {
    public abstract IEnumerator Execute(Unit unit);
}
