using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;

    void Start()
    {
        StartCoroutine("CoSendPacket");
        hitBox = GetComponent<HitBox>();
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Interaction();
    }

    

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z - 10);
    }

    public void Interaction()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                Debug.Log(item.name);
                Debug.Log(item.value);
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;
                Debug.Log(weaponIndex);
                Debug.Log(hasWeapons[weaponIndex]);
                //Destroy(nearObject);
            }
        }
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        //meleeArea.enabled = true;
        hitBox.meleeArea.enabled = true;
        //trailEffect.enabled = true;

        yield return new WaitForSeconds(0.7f);
        //meleeArea.enabled = false;
        hitBox.meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        //trailEffect.enabled = false;
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (wDown)
                Debug.Log("wDown True");

            if (moveVec.x == 1) // 오른쪽 : playerDir 1
                dir = 1;
            else if (moveVec.x == -1) // 왼쪽
                dir = 2;
            else if (moveVec.z == 1) // 위로 : playerDir 2
                dir = 3;
            else if (moveVec.z == -1) // 뒤로
                dir = 4;
            else if (moveVec.x > 0 && moveVec.z > 0) // 오른쪽 위
                dir = 5;
            else if (moveVec.x > 0 && moveVec.z < 0) // 오른쪽 뒤
                dir = 6;
            else if (moveVec.x < 0 && moveVec.z > 0) // 왼쪽 위
                dir = 7;
            else if (moveVec.x < 0 && moveVec.z < 0) // 왼쪽 뒤
                dir = 8;
            else
                dir = 0;

            C_Move movePacket = new C_Move();
            movePacket.playerIndex = 0;
            movePacket.playerDir = dir;
            movePacket.hp = hp;
            movePacket.posX = transform.position.x;
            movePacket.posY = transform.position.y;
            movePacket.posZ = transform.position.z;
            movePacket.wDown = wDown;
            movePacket.isJump = jDown;
            _network.Send(movePacket.Write());
        }
    }
}