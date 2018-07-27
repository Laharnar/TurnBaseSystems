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
            if (true) {
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
        }
        i--;
        
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
    private static void Test(string context, Vector3 result, Vector3 expected) {
        if (result != expected) {
            Debug.LogError("Test ("+context+") failed: expected:"+expected+" r:"+result);
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
