using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; } = new MainMenu();

    NetworkManager _network;

    public int charType = 0;

    public InputField playerIDInput;
    private string playerID = NetworkManager.Instance.id;

    

     void Awake()
    {
        //playerID = playerIDInput.GetComponent<InputField>().text;
    }
    void Start()
    {
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        playerID = playerIDInput.GetComponent<InputField>().text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickNewGame()  // 로그인 버튼
    {
        playerID = playerIDInput.text + '\0';
        C_Chat loginPacket = new C_Chat();
        loginPacket.chat = playerID;
        ArraySegment<byte> segment = loginPacket.Write();
        _network.Send(segment);
    }

    public void OnClickMakeRoom()
    {
        SceneManager.LoadScene("Room");
    }

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("InGame");
    }

    public void OnClickExitRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void OnClickMyInfo()
    {
        Debug.Log("내 정보");
    }

    public void OnClickNotice()
    {
        Debug.Log("공지사항");
    }

    public void OnClickSelectPower()
    {
        charType = 1;
        Debug.Log("파워");
        Debug.Log(charType);
        SceneManager.LoadScene("InGame");
        C_CREATE_PLAYER c_enterPacket = new C_CREATE_PLAYER();
        c_enterPacket.type = charType;
        c_enterPacket.nickname = playerID;
        ArraySegment<byte> segment = c_enterPacket.Write();
        _network.Send(segment);
    }

    public void OnClickSelectSpeed()
    {
        charType = 2;
        //Debug.Log("스피드");
        //Debug.Log(charType);
        SceneManager.LoadScene("InGame");
        C_CREATE_PLAYER c_enterPacket = new C_CREATE_PLAYER();
        c_enterPacket.type = charType;
        c_enterPacket.nickname = playerID;
        ArraySegment<byte> segment = c_enterPacket.Write();
        _network.Send(segment);
    }

    public void OnClickSelectShield()
    {
        charType = 3;
        Debug.Log("쉴드");
        Debug.Log(charType);
        SceneManager.LoadScene("InGame");
        C_CREATE_PLAYER c_enterPacket = new C_CREATE_PLAYER();
        c_enterPacket.type = charType;
        c_enterPacket.nickname = playerID;
        ArraySegment<byte> segment = c_enterPacket.Write();
        _network.Send(segment);
    }


    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
