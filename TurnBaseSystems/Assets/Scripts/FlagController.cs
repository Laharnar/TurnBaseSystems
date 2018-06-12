using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlagController {
    public List<Unit> units = new List<Unit>();
    public bool turnDone  = false;

    /// <summary>
    /// UI that should be active when player selects it. -- temp solution
    /// </summary>
    public Transform ui;

    public abstract IEnumerator FlagUpdate();
}
