using System;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour {

    static GameManager instance;

    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<GameManager>();
                if (instance == null) {
                    instance = new GameObject().AddComponent<GameManager>();
                }
                instance.Init();
            }
            return instance;
        }
        private set { instance = value; }
    }

    public CombatCam combatCam;
    [Header("Combat grid colors")]
    public Color globalColor = Color.black;
    public Color moveColor = Color.black;
    public Color attackColor = Color.black;
    public Color allySelectColor = Color.black;
    public Color enemySelectColor = Color.black;
    public Color aoeColor = Color.black;

    List<object> freeManagers;
    List<MonoBehaviour> monoManagers;
    List<MonoBehaviour> enumManagers;

    Dictionary<Type, object> fastAccess;

    bool init = false;
    public ColorSettings colorSettings;

    private void Awake() {
    }

    private void Init() {
        if (init) return;
        init = true;

        Instance = this;

        freeManagers = new List<object>();
        monoManagers = new List<MonoBehaviour>();
        enumManagers = new List<MonoBehaviour>();
        fastAccess = new Dictionary<Type, object>();
    }

    public void RegisterManager(object t, int i) {
        if (t == null ||i<0 || i> 2) {
            Debug.Log("Registration failed "+ t + " "+i);
            return;
        }
        if (freeManagers == null || monoManagers == null || enumManagers == null || fastAccess == null) {
            Init();
        }

        if (i==0) {
            freeManagers.Add(t);
        }
        if (i == 1) {
            monoManagers.Add(t as MonoBehaviour);
        }
        if (i == 2) {
            enumManagers.Add(t as MonoBehaviour);
        }
        fastAccess.Add(t.GetType(), t);
    }

    public T GetMonoManager<T>() where T: MonoBehaviour {
        if (fastAccess.ContainsKey(typeof(T)))
            return fastAccess[typeof(T)] as T;
        for (int i = 0; i < monoManagers.Count; i++) {
            if (monoManagers[i].GetType() == typeof(T)) {
                return monoManagers[i] as T;
            }
        }
        return null;
    }

    public T GetFreeManager<T>() where T: class{
        if (fastAccess.ContainsKey(typeof(T)))
            return fastAccess[typeof(T)] as T;
        for (int i = 0; i < freeManagers.Count; i++) {
            if (freeManagers[i].GetType() == typeof(T)) {
                fastAccess.Add(typeof(T), freeManagers[i]);
                return freeManagers[i] as T;
            }
        }
        return null;
    }

    public T GetEnumManager<T>() where T : MonoBehaviour {
        if (fastAccess.ContainsKey(typeof(T)))
            return fastAccess[typeof(T)] as T;
        for (int i = 0; i < enumManagers.Count; i++) {
            if (enumManagers[i].GetType() == typeof(T)) {
                fastAccess.Add(typeof(T), enumManagers[i]);
                return enumManagers[i] as T;
            }
        }
        return null;
    }

    public T GetManager<T>() where T: class {
        if (fastAccess.ContainsKey(typeof(T)))
            return fastAccess[typeof(T)] as T;
        return null;
    }
}