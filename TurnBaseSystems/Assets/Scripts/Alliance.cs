[System.Serializable]
public class Alliance {
    public int allianceId = 0;

    public bool IsPlayer { get { return allianceId == 0; } }

    public bool CanInteract(Alliance other) {
        return other.allianceId != allianceId;
    }
}
