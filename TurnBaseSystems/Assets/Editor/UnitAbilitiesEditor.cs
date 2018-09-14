using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitAbilities),true)]
public class UnitAbilitiesEditor : UnityEditor.Editor {

    int selectedForEditing = -1;

    public override void OnInspectorGUI() {
        UnitAbilities source = target as UnitAbilities;
        EditorGUI.BeginChangeCheck();

        // extract swap class[2/2]
        base.OnInspectorGUI();
        if (!Application.isPlaying) {
            for (int i = 0; i < source.additionalAbilities2.Count; i++) {
                // title
                if (source.additionalAbilities2[i] == null) {
                    GUILayout.Label("Null value");
                } else {
                    GUILayout.Label(source.additionalAbilities2[i].o_attackName);
                }

                if (GUILayout.Button("Select")) {
                    selectedForEditing = i;
                }
                if (selectedForEditing != -1 && i != selectedForEditing && GUILayout.Button("Swap " + i + " here")) {
                    AttackData2 x = source.additionalAbilities2[selectedForEditing];
                    source.additionalAbilities2[selectedForEditing] = source.additionalAbilities2[i];
                    source.additionalAbilities2[i] = x;
                    selectedForEditing = -1;
                }
            }
        }
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
            //data.attackFunction = AttackBaseType.GetAttackType(data.attackType_EditorOnly);
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