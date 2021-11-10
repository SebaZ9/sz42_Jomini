using ProtoMessageClient;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LogIn : Controller
{
    [SerializeField] private InputField txtUsername;
    [SerializeField] private InputField txtPassword;
    [SerializeField] private Text lblErrorMessage;
    [SerializeField] private Button btnLogIn;
    [SerializeField] private Button btnQuit;

    // Start is called before the first frame update
    void Start()
    {
        if(tclient == null) {
            Initialise();
        }

        btnLogIn.onClick.AddListener(BtnLogIn);
        btnQuit.onClick.AddListener(BtnQuit);

        if(string.IsNullOrWhiteSpace(globalString)) {
            lblErrorMessage.text = "";
        }
        else {
            lblErrorMessage.text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + globalString;
        }
    }

    private void BtnLogIn()
    {
        string username = txtUsername.text;
        string password = txtPassword.text;
        if(Login(username, password, out string reason)) {
            GoToScene(SceneName.Map);
        }
        else {
            Debug.Log(reason);
            lblErrorMessage.text = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + reason;
        }
    }

    private void BtnQuit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
