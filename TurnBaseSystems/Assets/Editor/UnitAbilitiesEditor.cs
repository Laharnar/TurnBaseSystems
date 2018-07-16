using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitAbilities),true)]
public class UnitAbilitiesEditor : UnityEditor.Editor {

    AttackData data;
    public AttackBaseType attackFunction;

    public override void OnInspectorGUI() {
        UnitAbilities source = target as UnitAbilities;
        if (Application.isPlaying) {
            for (int i = 0; i < source.additionalAbilities.Count; i++) {
                source.additionalAbilities[i] = ShowAttackData(source.additionalAbilities[i]);
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            base.OnInspectorGUI();
            return;
        }

        EditorGUI.BeginChangeCheck();
        data = ShowAttackData(data);

        if (GUILayout.Button("+")) {
            source.additionalAbilities.Add(new AttackData() {
                o_attackName = data.o_attackName,
                actionCost = data.actionCost,
                animData = new AttackAnimationInfo() {
                    animLength = data.animData.animLength,
                    animTrigger = data.animData.animTrigger,
                    useInfo = data.animData.useInfo
                },
                attackMask = data.attackMask,
                attackType = data.attackType,
                attackType_EditorOnly = data.attackType_EditorOnly,
                requiresUnit = data.requiresUnit,
                attackFunction = attackFunction
            });
        }
        EditorGUILayout.Separator();

        EditorGUI.indentLevel++;
        AttackData[] attacks = source.GetNormalAbilities();
        for (int i = 0; i < attacks.Length; i++) {
            attacks[i] = ShowAttackData(attacks[i]);
            EditorGUILayout.Space();
        }
        source.SaveNewAbilities(attacks);
        for (int i = 0; i < source.additionalAbilities.Count; i++) {
            source.additionalAbilities[i] = ShowAttackData(source.additionalAbilities[i]);
            if (GUILayout.Button("Remove " + (i + 1) + " (" + source.additionalAbilities[i].o_attackName + ")")) {
                source.additionalAbilities.RemoveAt(i);
                i--;
                continue;
            }
            EditorGUILayout.Space();
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Changed character");
        }
        EditorFix.SetObjectDirty(target);


    }

    private AttackData ShowAttackData(AttackData data) {
        if (data == null) {
            data = new AttackData() { animData = new AttackAnimationInfo() { animTrigger = "Attack"}, actionCost=1, attackType = "Normal", requiresUnit=false, o_attackName="noname", attackType_EditorOnly = AttackType.SingleTarget };
        }
        data.o_attackName= EditorGUILayout.TextField("Name", data.o_attackName) ;
        data.attackMask = EditorGUILayout.ObjectField("Mask", data.attackMask, typeof(GridMask), false) as GridMask;
        data.attackType = EditorGUILayout.TextField("Attack type", data.attackType);
        data.actionCost = EditorGUILayout.IntField("Action cost", data.actionCost);
        data.requiresUnit = EditorGUILayout.Toggle("Requires unit target", data.requiresUnit);
        AttackType last = data.attackType_EditorOnly;
        data.attackType_EditorOnly = (AttackType)EditorGUILayout.EnumPopup(data.attackType_EditorOnly);
        if (last != data.attackType_EditorOnly || data.attackFunction == null) {
            data.attackFunction = AttackBaseType.GetAttackType(data.attackType_EditorOnly);
        }

        data.animData.useInfo = EditorGUILayout.BeginToggleGroup("Animations", data.animData.useInfo);
        data.animData.animTrigger = EditorGUILayout.TextField("Animation trigger", data.animData.animTrigger);
        data.animData.animLength = EditorGUILayout.FloatField("Animation length", data.animData.animLength);
        EditorGUILayout.EndToggleGroup();

        data = DrawByType(data);

        return data;
    }

    AttackData DrawByType(AttackData data) {
        if (data == null || data.attackFunction == null) {
            Debug.Log("err null "+ (data == null) + " "+ (data.attackFunction == null));
            return data;
        }
        BindingFlags flags = BindingFlags.Public |
             BindingFlags.Instance;
        FieldInfo[] variables = data.attackFunction.GetType().GetFields(flags);
        for (int i = 0; i < variables.Length; i++) {
            object v = variables[i].GetValue(data.attackFunction);
            if (variables[i].FieldType == typeof(float)) {
                v= EditorGUILayout.FloatField(variables[i].Name, (float)v);
            }
            else if (variables[i].FieldType == typeof(string)) {
                v = EditorGUILayout.TextField(variables[i].Name, (string)v);
            }
            else if (variables[i].FieldType == typeof(int)) {
                v = EditorGUILayout.IntField(variables[i].Name, (int)v);
            }
            variables[i].SetValue(data.attackFunction, v);
        }
        /*
        FieldInfo[] fields = data.attackFunction.GetType().GetFields(flags);
        foreach (FieldInfo fieldInfo in fields) {
            Debug.Log("Field: " + fieldInfo.Name + " "+fieldInfo.FieldType + " " + fieldInfo.MemberType);
        }
        PropertyInfo[] properties = data.attackFunction.GetType().GetProperties(flags);
        foreach (PropertyInfo propertyInfo in properties) {
            Debug.Log("Property: " + propertyInfo.Name);
        }*/
        return data;
    }
}