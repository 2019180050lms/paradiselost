using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterInGame : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject obj;
    void Start()
    {
        
    }

     void Awake()
    {
        CreatePlayer("otherPlayer");
        
    }

    void CreatePlayer(string objName)
    {
        GameObject Instance = Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        Instance.name = objName;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
