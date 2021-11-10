using ProtoMessageClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationUI : Controller
{
    [SerializeField] private Button btnMap;
    [SerializeField] private Button btnViewEntourage;
    [SerializeField] private Button btnViewLeader;
    [SerializeField] private Button btnViewMyArmies;
    [SerializeField] private Button btnViewMyFiefs;
    [SerializeField] private Button btnViewJournal;
    [SerializeField] private Button btnSwitchCharacter;
    [SerializeField] private Button btnEndSeason;
    [SerializeField] private Button btnLogOut;

    [SerializeField] private Text lblGameDate;
    [SerializeField] private Text lblRealTime;

    // Start is called before the first frame update
    public void Start()
    {
        btnMap.onClick.AddListener(BtnMap);
        btnViewEntourage.onClick.AddListener(BtnViewEntourage);
        btnViewLeader.onClick.AddListener(BtnViewLeader);
        btnViewMyArmies.onClick.AddListener(BtnViewMyArmies);
        btnViewMyFiefs.onClick.AddListener(BtnViewMyFiefs);
        btnViewJournal.onClick.AddListener(BtnViewJournal);
        btnSwitchCharacter.onClick.AddListener(BtnSwitchCharacter);
        btnEndSeason.onClick.AddListener(BtnEndSeason);
        btnLogOut.onClick.AddListener(BtnLogOut);

        string season = SeasonToString(protoClient.Season);
        lblGameDate.text = season + "\n" + protoClient.Year.ToString();
    }

    private void BtnEndSeason() {
        ProtoMessage reply = SeasonUpdate(tclient);
        if(reply.ResponseType == DisplayMessages.Success) {
            viewingListSelectAction = "Journal";
            GoToScene(SceneName.ViewingList);
        }
    }

    private void BtnLogOut() {
        tclient.LogOut();
        GoToScene(SceneName.LogIn);
    }

    private void BtnMap() {
        SceneManager.LoadScene("Map");
    }

    private void BtnSwitchCharacter() {
        viewingListSelectAction = "SwitchCharacter";
        GoToScene(SceneName.ViewingList);
    }

    private void BtnViewEntourage() {
        // chars to view
        ProtoGenericArray<ProtoCharacterOverview> entourageList = GetNPCList("Entourage", tclient);
        //foreach(ProtoCharacterOverview character in possibleBailiffs.fields) {

        //}
        characterList = entourageList;
        viewingListSelectAction = "Entourage";

        GoToScene(SceneName.ViewingList);
    }

    private void BtnViewJournal() {
        viewingListSelectAction = "Journal";
        GoToScene(SceneName.ViewingList);
    }

    private void BtnViewLeader() {
        characterToViewID = protoClient.playerChar.charID;
        GoToScene(SceneName.ViewCharacter);
    }

    private void BtnViewMyArmies() {
        viewingListSelectAction = "YourArmies";
        GoToScene(SceneName.ViewingList);
    }

    private void BtnViewMyFiefs() {
        viewingListSelectAction = "MyFiefs";
        GoToScene(SceneName.ViewingList);
    }

    // Update is called once per frame
    void Update()
    {
        lblRealTime.text = DateTime.Now.ToString("HH:mm:ss");
    }
}
