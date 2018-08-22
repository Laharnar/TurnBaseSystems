﻿[System.Serializable]
public class AttackAnimationInfo {
    public bool useInfo = false;
    public int useData = 0; // 0: trigger
    public string animTrigger;
    public string animBool;
    public bool animBoolValue = false;
    public string animFloat;
    public float animFloatValue = 0;

    /// <summary>
    /// Time, length of animation
    /// </summary>
    public float animLength;
}
