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
        //Debug.Log("새 게임");
        SceneManager.LoadScene("Lobby");
    }

    public void OnClickMakeRoom()
    {
        SceneManager.LoadScene("InGame");
    }

    public void OnClickMyInfo()
    {
        Debug.Log("내 정보");
    }

    public void OnClickNotice()
    {
        Debug.Log("공지사항");
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
