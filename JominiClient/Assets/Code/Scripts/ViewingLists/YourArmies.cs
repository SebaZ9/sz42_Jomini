using ProtoMessageClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YourArmies : Controller
{

    public Transform CanvasParent;
    public GameObject prefab;

    public Button btnHireTroops;
    public TMPro.TMP_InputField TroopAmount;
    public Text lblMessageForUser;

    // Start is called before the first frame update
    void Start()
    {

        btnHireTroops.onClick.AddListener(BtnHireTroops);

        ProtoMessage reply = ListArmies(tclient);
        if (reply.ResponseType == DisplayMessages.Success) {
            armyList = (ProtoGenericArray<ProtoArmyOverview>)reply;

            if (armyList.fields != null)
            {
                foreach(ProtoArmyOverview army in armyList.fields) {

                    string location = string.IsNullOrWhiteSpace(army.locationID) ? "Unknown" : fiefNames[army.locationID];

                    GameObject UIElement = Instantiate(prefab);
                    UIElement.transform.SetParent(CanvasParent);
                    UIElement.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText(army.ownerName);
                    UIElement.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().SetText(army.leaderName);
                    UIElement.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().SetText(army.armySize.ToString());
                    UIElement.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().SetText(location);
                    UIElement.transform.localScale = Vector3.one;

                    if (viewingListSelectAction.Equals("ArmiesInFief") || viewingListSelectAction.Equals("YourArmies")) {
                        UIElement.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { BtnViewArmy(army.armyID); });
                    }

                }
            }

        } else {
            Debug.Log("Failed to retrieve list of armies");
        }


    }

    private void BtnHireTroops()
    {
        ProtoMessage reply = HireTroops(int.Parse(TroopAmount.text), tclient);
        switch (reply.ResponseType)
        {
            case DisplayMessages.ErrorGenericMessageInvalid:
                {
                    DisplayMessageToUser("ErrorGenericMessageInvalid!");
                    break;
                }
            case DisplayMessages.ErrorGenericArmyUnidentified:
                {
                    DisplayMessageToUser("ErrorGenericArmyUnidentified!");
                    break;
                }
            case DisplayMessages.ErrorGenericUnauthorised:
                {
                    DisplayMessageToUser("ErrorGenericUnauthorised!");
                    break;
                }
            case DisplayMessages.ErrorGenericPoorOrganisation:
                {
                    DisplayMessageToUser("ErrorGenericPoorOrganisation!");
                    break;
                }
            case DisplayMessages.CharacterRecruitInsufficientFunds:
                {
                    DisplayMessageToUser("CharacterRecruitInsufficientFunds!");
                    break;
                }
            case DisplayMessages.CharacterRecruitOk:
                {
                    DisplayMessageToUser("CharacterRecruitOk!");
                    break;
                }
            case DisplayMessages.Success:
                {
                    GoToScene(SceneName.ViewArmiesList);
                    DisplayMessageToUser("Success!");
                    break;
                }
            default:
                {
                    DisplayMessageToUser("default!");
                    break;
                }
        }
    }

    private void DisplayMessageToUser(string message)
    {
        lblMessageForUser.text = "[" + DateTime.Now.ToString("h:mm:ss tt") + "] " + message;
    }

    private void BtnViewArmy(string armyID) {
        armyToViewID = armyID;
        GoToScene(SceneName.ViewArmy);
    }
}
