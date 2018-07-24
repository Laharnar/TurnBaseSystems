using UnityEngine;
public class BuildingManager:MonoBehaviour {
    public static BuildingManager m;

    public Transform wallPref;
    private void Awake() {
        m = this;
    }
    internal void CreateWall(GridItem target) {
        
        target.fillAsStructure= Instantiate(wallPref, target.worldPosition, new Quaternion()).GetComponent<Structure>();
        //target.GetComponent<GridItem>().AddEnvInteraction("Combustible", "Consumable");
    }
}

