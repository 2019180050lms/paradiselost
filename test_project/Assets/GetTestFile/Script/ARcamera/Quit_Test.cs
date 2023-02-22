namespace JiantBattle
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;


    public class Quit_Test : MonoBehaviour
    {
        public Button Test_Button;


        int scoreButton;
        int scorelmage;

        private void Awake()
        {
            Button bts = Test_Button.GetComponent<Button>();
            bts.onClick.AddListener(QuitButton);
        }

        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void QuitButton()
        {
            this.gameObject.SetActive(false);
            Debug.Log("Hello world");
        }

        private void OnDisable()
        {
            
        }
    }
}


