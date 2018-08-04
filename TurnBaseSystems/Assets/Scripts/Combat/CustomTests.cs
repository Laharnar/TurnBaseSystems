using System;
using System.Collections;
using UnityEngine;
public class CustomTests: MonoBehaviour {
    static CustomTests m;
    int i=4;
    public bool showPassed = false;
    private void Awake() {
        m = this;
    }
    private void Update() {
        if (i == 0) {
            if (false) {
                GridMask m = FlagManager.flags[1].units[0].abilities.move2.AttackMask;
                Vector3 r = new Vector3(-FlagManager.flags[1].units[0].abilities.move2.AttackMask.w / 2, 0, 0);

                Test("AiHelper.ClosestToTargetOverMask1", AiHelper.ClosestToTargetOverMask(new Vector3(), new Vector3(-100, 0, 0), m), r);
                Test("AiHelper.ClosestToTarget", AiHelper.ClosestToTarget(new Vector3(), new Vector3(-100, 0, 0), m), r);
                Print("Get mask positions", m.GetFreePositions(new Vector3()));
                Print("In mask", m.IsPosInMask(new Vector3(), new Vector3(0, 4, 0)));

                float[] distsToSource = new float[] { 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2 };
                float[] distsToTarget = new float[] { 3, 4, 5, 4, 1, 2, 3, 4, 5, 4, 3, 2 };
                Test("AiHelper.IndexOfClosestToTarget", AiHelper.IndexOfClosestToTarget(distsToTarget, distsToSource), 4);
            }
            if (false) {
                EmpowerAlliesData d1 = new EmpowerAlliesData() { shieldUp = 2 };
                EmpowerAlliesData d2 = new EmpowerAlliesData() { shieldUp = 5 };
                CombatStats comstat = new CombatStats();
                comstat.Increase(d1, CombatStatType.Armor, d1.shieldUp);
                Test("EmpowerAlliesData1", comstat.GetSum(CombatStatType.Armor), 2);
                comstat.Reduce(d1, CombatStatType.Armor, d1.shieldUp);
                Test("EmpowerAlliesData1", comstat.GetSum(CombatStatType.Armor), 0);

                comstat.Increase(d1, CombatStatType.Armor, d1.shieldUp);
                comstat.Increase(d1, CombatStatType.Armor, d1.shieldUp);
                comstat.Increase(d2, CombatStatType.Armor, d2.shieldUp);

                Test("EmpowerAlliesData2b", comstat.GetSum(CombatStatType.Armor), 9);
                comstat.Reduce(d1, CombatStatType.Armor, 1);
                Test("EmpowerAlliesData2c", comstat.GetSum(d1, 0, CombatStatType.Armor), 1);
                Test("EmpowerAlliesData2d", comstat.GetSum(d1, 1, CombatStatType.Armor), 2);
                Test("EmpowerAlliesData2d", comstat.GetSum(d2, CombatStatType.Armor), 5);
                Test("EmpowerAlliesData2e", comstat.GetSum(d1, CombatStatType.Armor), 3);

                Test("EmpowerAlliesData2f", comstat.GetSum(CombatStatType.Armor), 8);

                comstat.Set(d1, CombatStatType.Armor, d1.shieldUp * 7);
                Test("EmpowerAlliesData3a", comstat.GetSum(CombatStatType.Armor), 14);
                Test("EmpowerAlliesData3c", comstat.GetSum(d1, 0, CombatStatType.Armor), 1);
                Test("EmpowerAlliesData3d", comstat.GetSum(d1, 1, CombatStatType.Armor), 2);
                Test("EmpowerAlliesData3e", comstat.GetSum(d2, CombatStatType.Armor), 5);
                Test("EmpowerAlliesData3f", comstat.GetSum(d1, 2, CombatStatType.Armor), 6);

                comstat.Reduce(CombatStatType.Armor, 13);
                Test("EmpowerAlliesData4a", comstat.GetSum(CombatStatType.Armor), 1);
                Test("EmpowerAlliesData4c", comstat.GetSum(d1, 0, CombatStatType.Armor), 1);
                Test("EmpowerAlliesData4d", comstat.GetSum(d1, 1, CombatStatType.Armor), 0);
                Test("EmpowerAlliesData4e", comstat.GetSum(d2, CombatStatType.Armor), 0);
                Test("EmpowerAlliesData4f", comstat.GetSum(d1, 2, CombatStatType.Armor), 0);

            }
            if (false) {
                GameObject witchObj= GameObject.Find("UNIT _ consumer _ Infested Witch(Clone)");
                // empower test
                Unit witch = witchObj.GetComponent<Unit>();
                AttackData2 atk = witch.abilities.additionalAbilities2[1];
                CombatStats comstat = witch.stats;
                witch.OnTurnStart();
                witch.AttackAction2(witch.transform.position, witch.abilities.additionalAbilities2[1]);

                Test("EmpowerShieldBuffWorks1a", comstat.GetSum(CombatStatType.Armor), 3);
                witch.GetDamaged(1);
                Test("EmpowerShieldBuffWorks1b", comstat.GetSum(CombatStatType.Armor),2);
                witch.GetDamaged(2);
                Test("EmpowerShieldBuffWorks1c", comstat.GetSum(CombatStatType.Armor),0);
                Test("EmpowerShieldBuffWorks1f", comstat.GetSum(CombatStatType.Hp), 4);

                witch.OnTurnStart();
                witch.AttackAction2(witch.transform.position, witch.abilities.additionalAbilities2[1]);
                comstat = witch.stats;

                Test("EmpowerShieldBuffWorks2a", comstat.GetSum(CombatStatType.Armor), 3);
                Test("EmpowerShieldBuffWorks2a2", comstat.GetSum(CombatStatType.Hp), 4);
                witch.GetDamaged(2);
                Test("EmpowerShieldBuffWorks2b", comstat.GetSum(CombatStatType.Armor), 1);
                witch.GetDamaged(2);
                Test("EmpowerShieldBuffWorks2c", comstat.GetSum(CombatStatType.Armor), 0);
                Test("EmpowerShieldBuffWorks2d", comstat.GetSum(CombatStatType.Hp), 3);
                witch.GetDamaged(1);
                Test("EmpowerShieldBuffWorks2c", comstat.GetSum(CombatStatType.Armor), 0);
                Test("EmpowerShieldBuffWorks2d", comstat.GetSum(CombatStatType.Hp), 2);

                witch.OnTurnStart();
                witch.AttackAction2(witch.transform.position, witch.abilities.additionalAbilities2[1]);

                Test("EmpowerShieldBuffWorks3c", comstat.GetSum(CombatStatType.Armor), 3);
                Test("EmpowerShieldBuffWorks3d", comstat.GetSum(CombatStatType.Hp), 3);

            }
            if (false) {
                GameObject witchObj = GameObject.Find("UNIT _ consumer _ Infested Witch(Clone)");
                // empower test
                Unit witch = witchObj.GetComponent<Unit>();
                AttackData2 atk = witch.abilities.additionalAbilities2[1];
                CombatStats comstat = witch.stats;
                int x = 3;
                witch.OnTurnStart();
                witch.AddShield(atk.buff, atk.buff.armorAmt);
                Test("Add shield1a", comstat.GetSum(CombatStatType.Armor), 3);
                BuffManager.Register(witch, atk.buff);
                BuffManager.ConsumeBuffs(1);
                //-witch.AddShield(atk.buff, -atk.buff.armorAmt);
                Test("Add shield1b", comstat.GetSum(CombatStatType.Armor), 0);
                //witch.AttackAction2(witch.transform.position, witch.abilities.additionalAbilities2[1]);

            }
            if (false) {
                GameObject witchObj = GameObject.Find("UNIT _ consumer _ Infested Witch(Clone)");
                // empower test
                Unit witch = witchObj.GetComponent<Unit>();
                AttackData2 atk = witch.abilities.additionalAbilities2[1];
                CombatStats comstat = witch.stats;
                int x = 3;
                witch.OnTurnStart();
                witch.AddShield(atk.buff, atk.buff.armorAmt);
                Test("Add shield1a", comstat.GetSum(CombatStatType.Armor), 3);
                BuffManager.Register(witch, atk.buff);
                BuffManager.ConsumeBuffs(1);
                //witch.AddShield(atk.buff, -atk.buff.armorAmt);
                Test("Add shield1b", comstat.GetSum(CombatStatType.Armor), 0);
                //witch.AttackAction2(witch.transform.position, witch.abilities.additionalAbilities2[1]);

            }
            StartCoroutine(SlowTests());
            
        }
        i--;
        
    }

    private IEnumerator SlowTests() {
        if (false) {
            GameObject dekuriongo = GameObject.Find("DEKURIONMelleUNIT");
            GameObject mellego = GameObject.Find("MelleUNIT (7)");
            Unit dekurion = dekuriongo.GetComponent<Unit>();
            Unit melee = mellego.GetComponent<Unit>();
            CombatStats comstat1 = dekurion.stats;
            CombatStats comstat2 = melee.stats;
            dekuriongo.transform.position = new Vector3(0, 1,0);
            mellego.transform.position = new Vector3(1, 0, 0);
            CombatManager.m.OnTurnStart(1);
            Test("Auras1a", comstat1.GetSum(CombatStatType.Armor), 1);
            Test("Auras1b", comstat2.GetSum(CombatStatType.Armor), 1);
            yield return new WaitForSeconds(2);
            dekurion.abilities.additionalAbilities2[1].aura.EffectArea(dekurion.transform.position, dekurion);
            dekurion.abilities.additionalAbilities2[1].aura.EffectArea(dekurion.transform.position, dekurion);
            dekurion.abilities.additionalAbilities2[1].aura.EffectArea(dekurion.transform.position, dekurion);
            Test("Auras6a", comstat1.GetSum(CombatStatType.Armor), 4);
            yield return new WaitForSeconds(2);

            dekurion.abilities.additionalAbilities2[1].aura.DeEffectArea(dekurion.transform.position, dekurion,true);
            dekurion.abilities.additionalAbilities2[1].aura.DeEffectArea(dekurion.transform.position, dekurion,true);
            dekurion.abilities.additionalAbilities2[1].aura.DeEffectArea(dekurion.transform.position, dekurion,true);
            Test("Auras6b", comstat1.GetSum(CombatStatType.Armor), 1);

            yield return new WaitForSeconds(2);

            EmpowerAlliesData aura1 = dekurion.abilities.additionalAbilities2[1].aura;
            dekurion.transform.position = new Vector3(-10, 1, 0);
            CombatManager.OnUnitExecutesMoveAction(new Vector3(), new Vector3(-10, 1, 0), dekurion);
            Test("AurasSourceMoveOut2a", comstat1.GetSum(CombatStatType.Armor), 1);
            Test("AurasSourceMoveOut2b", comstat2.GetSum(CombatStatType.Armor), 0);
            yield return new WaitForSeconds(2);

            melee.transform.position = new Vector3(-9, 0, 0);
            CombatManager.OnUnitExecutesMoveAction(new Vector3(1, 0, 0), new Vector3(-9, 0, 0), melee);
            Test("NonAuraMoveINAura3a", comstat1.GetSum(CombatStatType.Armor), 1);
            Test("NonAuraMoveINAura3b", comstat2.GetSum(CombatStatType.Armor), 1);
            yield return new WaitForSeconds(2);

            melee.transform.position = new Vector3(0, 0, 0);
            CombatManager.OnUnitExecutesMoveAction(new Vector3(-9,0,0), new Vector3(0, 0, 0), melee);
            Test("NonAuraMoveOut4a", comstat1.GetSum(CombatStatType.Armor), 1);
            Test("NonAuraMoveOut4b", comstat2.GetSum(CombatStatType.Armor), 0);
            yield return new WaitForSeconds(2);

            dekurion.transform.position = new Vector3(-1, 1, 0);
            CombatManager.OnUnitExecutesMoveAction(new Vector3(-10, 1, 0), new Vector3(-1, 1, 0), dekurion);
            Test("AuraMoveIn3a", comstat1.GetSum(CombatStatType.Armor), 1);
            Test("AuraMoveIn3b", comstat2.GetSum(CombatStatType.Armor), 1);
            yield return new WaitForSeconds(2);
        }

        if (false) {
            GridDisplay.ClearAll();

            GridDisplay.RemakeGrid();
            //Test("Grids", GridDisplay.layers[1].items.Count, 0);
            GridDisplay.SetUpGrid(new Vector3(), 0, 1, GridManager.m.maskTemplates[2]);
            GridDisplay.RemakeGrid();
            Test("Grids1", GridDisplay.layers[0].items.Count, 24);
            Test("Grids1", GridDisplay.flattened.Count, 24);
            GridDisplay.HideGrid(new Vector3(), 0, GridManager.m.maskTemplates[2]);
            Test("Grids1", GridDisplay.layers[0].items.Count, 0);
            GridDisplay.RemakeGrid();
            Test("Grids1", GridDisplay.flattened.Count, 0);

            //Test("Grids", GridDisplay.layers[1].items.Count, 0);

            GridDisplay.SetUpGrid(new Vector3(), 0, 1, GridManager.m.maskTemplates[2]);
            GridDisplay.SetUpGrid(new Vector3(2,0,0), 1, 4, GridManager.m.maskTemplates[0]);
            GridDisplay.RemakeGrid();
            Test("Grids2a", GridDisplay.layers[0].items.Count, 24);
            Test("Grids2b", GridDisplay.layers[1].items.Count, 4);
            Test("Grids2c", GridDisplay.flattened.Count, 24);
            yield return new WaitForSeconds(3);
            GridDisplay.HideGrid(new Vector3(), 0, GridManager.m.maskTemplates[2]);
            Test("Grids2d", GridDisplay.layers[0].items.Count, 0);
            //Test("Grids2e", GridDisplay.layers[1].items.Count, 4);
            GridDisplay.MoveGrid(new Vector3(2,0,0), new Vector3(-2, 0, 0), 1, 4, GridManager.m.maskTemplates[0]);
            GridDisplay.RemakeGrid();
            //Test("Grids2f", GridDisplay.flattened.Count, 4);

        }
        if (false) {
            Test("calc 1", Mathf.CeilToInt(4f * (1-0.9f)), 1);
            
        }
        if (true) {
            GameObject nukergo = GameObject.Find("UNIT _ consumer _ NukeMage");
            GameObject mellego = GameObject.Find("MelleUNIT (7)");
            GameObject mellego1 = GameObject.Find("MelleUNIT (6)");
            Unit nuker = nukergo.GetComponent<Unit>();
            Unit melee = mellego.GetComponent<Unit>();
            Unit melee1 = mellego1.GetComponent<Unit>();
            CombatStats comstat1 = nuker.stats;
            CombatStats comstat2 = melee.stats;
            CombatStats comstat3 = melee1.stats;
            nukergo.transform.position = new Vector3(0, 5, 0);
            mellego.transform.position = new Vector3(0, 0, 0);
            mellego1.transform.position = new Vector3(0, -3, 0);
            nuker.GetDamaged(3);
            nuker.AddCharges(null, 1);
            Test("get pierced1a", nuker.charges, 2);
            CombatManager.m.OnTurnStart(0);
            CurrentActionData actionData = new CurrentActionData() {
                attackedSlot = new Vector3(0, 0, 0),
                attackStartedAt = nuker.snapPos,
                sourceExecutingUnit = nuker
            };
            PierceAtkData atkD = nuker.abilities.additionalAbilities2[1].pierce;
            CombatInfo.attackingUnit = nuker;
            CombatInfo.currentActionData = actionData;
            CombatInfo.activeAbility = nuker.abilities.additionalAbilities2[1];


            Unit[] units = atkD.GetUnitsPierced(melee);

            CombatManager.CombatAction(nuker, new Vector3(0, 0, 0), nuker.abilities.additionalAbilities2[1]);
            CombatManager.m.OnTurnStart(0);
            CombatManager.CombatAction(nuker, new Vector3(0, 0, 0), nuker.abilities.additionalAbilities2[1]);
            Test("get pierced1a", melee.dead, true);
            Test("get pierced1b", !melee1.dead, true);
            Test("get pierced1b", nuker.hp==2, true);
            yield return new WaitForSeconds(2);

        }

        yield return null;
    }

    private static void Print(string context, bool data) {
        Debug.Log(context+" "+data);
    }
    private static void Print(string context, Vector3[] data) {
        string s = context;
        for (int i = 0; i < data.Length; i++) {
            s += " " + data[i];
        }
        Debug.Log(s);
    }

    private static void Test(string context, Vector3[] result, Vector3[] expected) {
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
    private static void Test(string context, bool result, bool expected) {
        if (result != expected) {
            Debug.LogError("Test (" + context + ") failed: expected:" + expected + " r:" + result);
        } else {
            if (m && m.showPassed) {
                Debug.Log("Test (" + context + ") passed.");
            }
        }
    }
    private static void Test(string context, Vector3 result, Vector3 expected) {
        if (result != expected) {
            Debug.LogError("Test ("+context+") failed: expected:"+expected+" r:"+result);
        } else {
            if (m && m.showPassed) {
                Debug.Log("Test (" + context + ") passed.");
            }
        }
    }
    private static void Test(string context, int result, int expected) {
        if (result != expected) {
            Debug.LogError("Test (" + context + ") failed: expected:" + expected + " r:" + result);
        } else {
            if (m && m.showPassed) {
                Debug.Log("Test (" + context + ") passed.");
            }
        }
    }
}
