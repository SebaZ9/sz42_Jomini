using ProtoMessageClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignBailif : Controller
{
    public Transform CanvasParent;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {

        ProtoMessage reply = GetNPCList("Family Employ", tclient);
        if (reply.ResponseType == DisplayMessages.Success)
        {
            characterList = (ProtoGenericArray<ProtoCharacterOverview>)reply;

            foreach (ProtoCharacterOverview character in characterList.fields)
            {

                string location = string.IsNullOrWhiteSpace(character.locationID) ? "Unknown" : fiefNames[character.locationID];

                string sex = character.isMale ? "Male" : "Female";

                GameObject UIElement = Instantiate(prefab);
                UIElement.transform.SetParent(CanvasParent);
                UIElement.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText(character.charName);
                UIElement.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().SetText(sex);
                UIElement.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().SetText(character.role);
                UIElement.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().SetText(location);
                UIElement.transform.localScale = Vector3.one;

                UIElement.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { BtnAppointBailiff(character.charID); });

            }

        }
        else
        {
            Debug.Log("Failed to retrieve list of armies");
        }


    }

    private void BtnAppointBailiff(string charID)
    {
        ProtoMessage reply = AppointBailiff(charID, fiefToViewID, tclient);
        if (reply.ResponseType == DisplayMessages.Success)
        {
            GoToScene(SceneName.ViewFief);
        }
        else
        {
            Debug.Log("ERROR: Response type: " + reply.ResponseType.ToString());
        }
    }
}
