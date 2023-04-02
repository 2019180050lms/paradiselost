using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemyHP : MonoBehaviour
{

    public Image hpBar;

    BossEnemy bossEnemy;
    // Start is called before the first frame update
    void Start()
    {
        bossEnemy = GetComponent<BossEnemy>();
        InitHpBarSize();
        
    }

    void InitHpBarSize()
    {
        hpBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }
    // Update is called once per frame
    void Update()
    {
        hpBar.rectTransform.localScale = new Vector3((float)bossEnemy.curHealth / (float)bossEnemy.maxHealth, 1f, 1f);
    }
}
