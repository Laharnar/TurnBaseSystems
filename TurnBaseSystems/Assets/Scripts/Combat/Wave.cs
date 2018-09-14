using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Wave", menuName = "Waves/Wave", order = 1)]
public class Wave:ScriptableObject {
    public string description;
    public string additionalTrigger;

    public int[] spawnArea;
    public PrefabSet[] enemySet;

    //public int[] enemies;

    public void RunTrigger() {
        MissionManager.m.Invoke(additionalTrigger, 0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="w1"></param>
    /// <param name="w2"></param>
    /// <param name="swap">[description][trigger][spawn area]</param>
    public static void SwapData(Wave w1, Wave w2, int swap) {
        if (((swap >> 2) & 1) == 1) {
            string x = w1.description;
            w1.description = w2.description;
            w2.description = x;
        }
        if (((swap >> 1) & 1) == 1) {
            string x = w1.additionalTrigger;
            w1.additionalTrigger = w2.additionalTrigger;
            w2.additionalTrigger = x;
        }
        if  (((swap >> 0) & 1)  == 1) {
            int[] x = w1.spawnArea;
            w1.spawnArea = w2.spawnArea;
            w2.spawnArea = x;
            PrefabSet[] y = w1.enemySet;
            w1.enemySet = w2.enemySet;
            w2.enemySet = y;
        }
    }
}
