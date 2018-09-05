using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatTestingEnv:MonoBehaviour {
    public bool showPassed = false;
    public Text statusTxt;
    private void Start() {
        TestEval.showPassed = showPassed;
        RunAllTests();
    }

    public void RunAllTests() {
        this.StartCoroutine(RunTests(this));
    }

    public IEnumerator RunTests(MonoBehaviour script) {
        // yield return script.StartCoroutine(Test1_Auras());
        //yield return script.StartCoroutine(Test2_Buffs());
        yield return script.StartCoroutine(Test3_Human());
    }

    private IEnumerator Test3_Human() {
        SpawnReadyUnit(0, "Royal enforcer", new Vector3(0, 0));
        SpawnReadyUnit(1, "Zombie", new Vector3(1, 0));
        yield return null;

        Unit enforcer = SelectUnitByType(0, "Royal enforcer");
        Unit zombie = SelectUnitByType(1, "Zombie");
        if (zombie) {
            AttackSlot(zombie, SelectAbility(zombie, 1), new Vector3(0, 0), new CombatEventMask() { onAttack = true });
            AttackSlot(zombie, SelectAbility(zombie, 1), new Vector3(0, 0), new CombatEventMask() { onAttack = true });
            TestEval.Test("enforcer hp", enforcer.hp, 1);
        } else {
            MissingUnitError(zombie , "Zombie");
        }
        yield return null;
    }

    private void MissingUnitError(Unit zombie, string v) {
        Debug.Log("Missing unit "+v+ " "+zombie);
    }

    public IEnumerator Test2_Buffs() {
        Unit dekurion = SpawnReadyUnit(0, "Royal enforcer", new Vector3(0, 0));
        yield return null;
        Unit enforcer = SelectUnitByType(0, "Royal enforcer");
        AttackSlot(enforcer, SelectAbility(enforcer, "Protection"), new Vector3(0, 0), new CombatEventMask() { onAttack = true });
        // lasts 2 turns
        TestEval.Test("Protection amt 1", enforcer.temporaryArmor, 5);
        ForceEndTurn(0);
        TestEval.Test("Protection amt 2", enforcer.temporaryArmor, 5);
        ForceEndTurn(1);
        TestEval.Test("Protection amt 3", enforcer.temporaryArmor, 3);
        yield return null;
    }

    public IEnumerator Test1_Auras() {

        Unit dekurion = SpawnReadyUnit(1, "Captain", new Vector3(0, 0));
        Unit enemyGuard = SpawnReadyUnit(1, "Guard", new Vector3(1, 0));
        Unit priest = SpawnReadyUnit(0, "Priest", new Vector3(-1, 0));

        CombatEvents.OnTurnStart(Combat.Instance.flags[1]);
        dekurion.abilities.additionalAbilities2[1].aura.maxAuraStacks = 3;
        TestEval.Test("Auras1a", dekurion.temporaryArmor, 1);
        TestEval.Test("Auras1b", enemyGuard.temporaryArmor, 1);
        TestEval.Test("Auras1b", priest.temporaryArmor, 0);
        SetStatus("startup turn aura");
        yield return new WaitForSeconds(3);
        dekurion.abilities.additionalAbilities2[1].aura.EffectArea(dekurion.transform.position, dekurion);
        dekurion.abilities.additionalAbilities2[1].aura.EffectArea(dekurion.transform.position, dekurion);
        dekurion.abilities.additionalAbilities2[1].aura.EffectArea(dekurion.transform.position, dekurion);
        SetStatus("triple effect aura, limit 3");
        TestEval.Test("Auras6a", dekurion.temporaryArmor, 3);
        TestEval.Test("Auras6b", enemyGuard.temporaryArmor, 3);
        TestEval.Test("Auras6c", priest.temporaryArmor, 0);
        yield return new WaitForSeconds(2);

        dekurion.abilities.additionalAbilities2[1].aura.DeEffectArea(dekurion.transform.position, dekurion, true);
        dekurion.abilities.additionalAbilities2[1].aura.DeEffectArea(dekurion.transform.position, dekurion, true);
        TestEval.Test("Auras6b", dekurion.temporaryArmor, 1);
        TestEval.Test("Auras6b", enemyGuard.temporaryArmor, 1);
        TestEval.Test("Auras6b", priest.temporaryArmor, 0);
        SetStatus("double reduce aura");

        yield return new WaitForSeconds(2);

        EmpowerAlliesData aura1 = dekurion.abilities.additionalAbilities2[1].aura;
        dekurion.transform.position = new Vector3(-10, 1, 0);
        CombatEvents.ReapplyAuras(new Vector3(), new Vector3(-10, 1, 0), dekurion);
        TestEval.Test("AurasSourceMoveOut2a", dekurion.temporaryArmor, 1);
        TestEval.Test("AurasSourceMoveOut2b", enemyGuard.temporaryArmor, 0);
        yield return new WaitForSeconds(2);
        /*
        enemyGuard.transform.position = new Vector3(-9, 0, 0);
        CombatEvents.ReapplyAuras(new Vector3(1, 0, 0), new Vector3(-9, 0, 0), enemyGuard);
        TestEval.Test("NonAuraMoveINAura3a", dekurion.temporaryArmor, 1);
        TestEval.Test("NonAuraMoveINAura3b", enemyGuard.temporaryArmor, 1);
        yield return new WaitForSeconds(2);

        enemyGuard.transform.position = new Vector3(0, 0, 0);
        CombatEvents.ReapplyAuras(new Vector3(-9, 0, 0), new Vector3(0, 0, 0), enemyGuard);
        TestEval.Test("NonAuraMoveOut4a", dekurion.temporaryArmor, 1);
        TestEval.Test("NonAuraMoveOut4b", enemyGuard.temporaryArmor, 0);
        yield return new WaitForSeconds(2);

        dekurion.transform.position = new Vector3(-1, 1, 0);
        CombatEvents.ReapplyAuras(new Vector3(-10, 1, 0), new Vector3(-1, 1, 0), dekurion);
        TestEval.Test("AuraMoveIn3a", dekurion.temporaryArmor, 1);
        TestEval.Test("AuraMoveIn3b", enemyGuard.temporaryArmor, 1);*/
        yield return new WaitForSeconds(2);
    }

    private void SetStatus(string v) {
        if (statusTxt)
            statusTxt.text = "Status:"+v;
    }

    public static void SetupUnit(Unit unit, int alliance) {
        unit.flag.allianceId = alliance;
        unit.Init();
        unit.detection.detectedSomeone = true;
    }

    public Unit SpawnReadyUnit(int alliance, string code, Vector3 pos) {
        Transform t = CharacterLibrary.m.CreateInstance(code);
        SetupUnit(t.GetComponent<Unit>(), alliance);
        t.position = GridManager.SnapPoint(pos);
        return t.GetComponent<Unit>();
    }

    public void ForceEndTurn(int flagId) {
        CombatEvents.OnTurnEnd(Combat.Instance.flags[flagId]);
    }

    public void SetCurrentCombatState(Unit execuutingUnit) {
        AbilityInfo.ExecutingUnit = execuutingUnit;
    }

    public Unit SelectUnitByType(int flag, string code) {
        foreach (var unit in Combat.Instance.flags[flag].info.units) {
            if (unit.codename == code) {
                return unit;
            }
        }
        return null;
    }

    public Unit SelectUnitByFlag(int flag, int i) {
        return Combat.Instance.flags[flag].info.units[i];
    }

    public AttackData2 SelectAbility(Unit unit, string ability) {
        foreach (var item in unit.abilities.GetNormalAbilities()) {
            if (item.o_attackName == ability) {
                return item;
            }
        }
        return null;
    }
    public AttackData2 SelectAbility(Unit unit, int ability) {
        return unit.abilities.GetNormalAbilities()[ability];
    }

    public bool AttackSlot(Unit unit, AttackData2 ablity, Vector3 slot, CombatEventMask mask) {
        SetCurrentCombatState(unit);
        unit.lastAttackParserPassed = false;
        AbilityInfo inf = new AbilityInfo(unit, slot, ablity) {
            activator = mask
        };
        unit.AttackAction(inf, true);
        return unit.lastAttackParserPassed;
    }
}

public static class TestEval {
    public static bool showPassed = false;

    public static void Print(string context, bool data) {
        Debug.Log(context+" "+data);
    }
    public static void Print(string context, Vector3[] data) {
        string s = context;
        for (int i = 0; i < data.Length; i++) {
            s += " " + data[i];
        }
            Debug.Log(s);
    }

    public static void Test(string context, Vector3[] result, Vector3[] expected) {
        if (result.Length != expected.Length) {
            Debug.LogError("Test (" + context + ") failed: different length expected:" + expected.Length + " r:" + result.Length);
            return;
        }
        for (int i = 0; i < result.Length && i< expected.Length; i++) {
            if (result[i] != expected[i]) {
                Debug.LogError("Test (" + context + ") failed at ["+i+"]: expected:" + expected + " r:" + result);
            }
        }
    }

    public static void Test(string context, bool result, bool expected) {
        if (result != expected) {
            Debug.LogError("Test (" + context + ") failed: expected:" + expected + " r:" + result);
        } else {
            if (showPassed) {
                Debug.Log("Test (" + context + ") passed.");
            }
        }
    }
    public static void Test(string context, Vector3 result, Vector3 expected) {
        if (result != expected) {
            Debug.LogError("Test ("+context+") failed: expected:"+expected+" r:"+result);
        } else {
            if (showPassed) {
                Debug.Log("Test (" + context + ") passed.");
            }
        }
    }
    public static void Test(string context, int result, int expected) {
        if (result != expected) {
            Debug.LogError("Test (" + context + ") failed: expected:" + expected + " r:" + result);
        } else {
            if (showPassed) {
                Debug.Log("Test (" + context + ") passed.");
            }
        }
    }
}
