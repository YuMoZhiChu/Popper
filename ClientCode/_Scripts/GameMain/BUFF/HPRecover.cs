using UnityEngine;
using System.Collections;

public class HPRecover : PlayerBuff {

    public float recoverRate = 0.5f;

    // 开始影响
    override public void StartBuff()
    {
        float hp = TPC.hpControl.currentHP + TPC.hpControl.HP * recoverRate;
        if (hp > TPC.hpControl.HP)
            hp = TPC.hpControl.HP;
        TPC.hpControl.currentHP = hp;
        Destroy(this);
    }

    // Use this for initialization
    void Start()
    {
        base.Init();
    }

}
