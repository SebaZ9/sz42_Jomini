using ProtoMessageClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmiesInFiefs : Controller
{

    public Transform CanvasParent;
    public GameObject prefab;

    public Text lblMessageForUser;
    public Text lblPageTitle;
    

    // Start is called before the first frame update
    void Start()
    {
        lblPageTitle.text = "Armies in " + fiefNames[fiefToViewID];

        ProtoMessage armiesInFief = ExamineArmiesInFief(fiefToViewID, tclient);
        if (armiesInFief.ResponseType == DisplayMessages.Success)
        {
            armyList = (ProtoGenericArray<ProtoArmyOverview>)armiesInFief;

            foreach (ProtoArmyOverview army in armyList.fields)
            {

                string location = string.IsNullOrWhiteSpace(army.locationID) ? "Unknown" : fiefNames[army.locationID];

                GameObject UIElement = Instantiate(prefab);
                UIElement.transform.SetParent(CanvasParent);
                UIElement.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText(army.ownerName);
                UIElement.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().SetText(army.leaderName);
                UIElement.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().SetText(army.armySize.ToString());
                UIElement.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().SetText(location);
                UIElement.transform.localScale = Vector3.one;

                UIElement.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { BtnViewArmy(army.armyID); });


            }

        }
        else
        {
            Debug.Log("Failed to retrieve list of armies");
        }

        

    }

    private void BtnViewArmy(string armyID)
    {
        armyToViewID = armyID;
        GoToScene(SceneName.ViewArmy);
    }

}
