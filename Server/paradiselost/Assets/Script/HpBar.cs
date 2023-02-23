using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private Slider hpbar;

    float imsi;
    Enemy enemy;

    public int curH = 100;
    public int maxH = 100;

    [SerializeField] GameObject m_goPrefab = null;

    List<Transform> m_objectList = new List<Transform>();
    List<GameObject> m_hpBarList = new List<GameObject>();

    Camera m_cam = null;


    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {

        curH = enemy.curHealth;
        maxH = enemy.maxHealth;

        hpbar.value = (float)curH / (float)maxH;

        m_cam = Camera.main;

        Invoke("FindEnemyLength", 1);

        
    }

    void FindEnemyLength()
    {
        GameObject[] t_objects = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < t_objects.Length; ++i)
        {
            m_objectList.Add(t_objects[i].transform);
            GameObject t_hpbar = Instantiate(m_goPrefab, t_objects[i].transform.position, Quaternion.identity, transform);
            m_hpBarList.Add(t_hpbar);
        }

    }

    // Update is called once per frame
    void Update()
    {

        //imsi = (float)enemy.curHealth / (float)enemy.maxHealth;

        


        for (int i = 0; i < m_objectList.Count; ++i)
        {
            m_hpBarList[i].transform.position = m_cam.WorldToScreenPoint(m_objectList[i].position + new Vector3(0, 3.5f, 0));
        }


        HandleHp();
    }

    private void HandleHp()
    {
        hpbar.value = (float)curH / (float)maxH;
        hpbar.value = Mathf.Lerp(hpbar.value, (float)curH / (float)maxH, Time.deltaTime * 10);
    }
}
