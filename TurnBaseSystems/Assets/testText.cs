using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testText : MonoBehaviour {

    public SeznamStevilk seznam;

    // Use this for initialization
    void Start () {
        seznam = new SeznamStevilk();
        for (int i = 0; i < seznam.tabela.Length; i++) {
            seznam.tabela[i] = i;
        }
	}
	// Update is called once per frame
	void Update () {
        Debug.Log(Sum(seznam.tabela));

        seznam.AddSize();
    }

    int Sum(int[] tabela) {
        int sum = 0;
        for (int i = 0; i < tabela.Length; i++) {
            sum += tabela[i];
        }
        return sum;
    }
    
}
[System.Serializable]// da lahko vidis cel seznam tudi direkt iz unity
public class SeznamStevilk {
    public int[] tabela = new int[1000];

    public void AddSize() {
        int[] tabelaTemporary = new int[tabela.Length + 1];
        for (int i = 0; i < tabela.Length; i++) {
            tabelaTemporary[i] = tabela[i];
        }
        tabelaTemporary[tabela.Length] = tabelaTemporary[tabela.Length-1]+1;
    }
}