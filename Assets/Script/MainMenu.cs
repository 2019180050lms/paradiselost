using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickNewGame()
    {
        //Debug.Log("�� ����");
        SceneManager.LoadScene("Lobby");
    }

    public void OnClickMakeRoom()
    {
        SceneManager.LoadScene("InGame");
    }

    public void OnClickMyInfo()
    {
        Debug.Log("�� ����");
    }

    public void OnClickNotice()
    {
        Debug.Log("��������");
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
