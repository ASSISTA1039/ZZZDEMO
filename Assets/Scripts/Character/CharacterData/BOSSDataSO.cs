using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSDataSO : CharacterDataBaseSO
{
    //ʧ��ֵ
    public float MaxUnbalance;
    //��ǰ����ֵ
    public float Unbalance;

    void Start()
    {
        //��ʼ��Ѫ��
        //maxHealth = 50000f;
        health = maxHealth;

        Unbalance = MaxUnbalance;


    }

    void Update()
    {

    }
}
