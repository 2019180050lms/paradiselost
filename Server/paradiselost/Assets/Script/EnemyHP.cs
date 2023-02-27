using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{

    public Image hpBar;

    Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        InitHpBarSize();
        
    }

    void InitHpBarSize()
    {
        hpBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }
    // Update is called once per frame
    void Update()
    {
        hpBar.rectTransform.localScale = new Vector3((float)enemy.curHealth / (float)enemy.maxHealth, 1f, 1f);
    }
}
