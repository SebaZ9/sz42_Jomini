using ProtoMessageClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewCharacters : Controller
{
    public Transform CanvasParent;
    public GameObject prefab;
    public Text lblPageTitle;

    // Start is called before the first frame update
    void Start()
    {

        lblPageTitle.text = "Persons present in the " + globalString + " of " + fiefNames[fiefToViewID];

        ProtoMessage reply = ListCharsInMeetingPlace(globalString, protoClient.activeChar.charID, tclient);
        if (reply.ResponseType == DisplayMessages.Success)
        {
            characterList = (ProtoGenericArray<ProtoCharacterOverview>)reply;
            if(characterList.fields != null)
            {
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

                    UIElement.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { BtnSwitchCharacter(character.charID); });
                    Debug.Log($"{character.charName} {character.charID}");

                }
            }
            

        }
        else
        {
            Debug.Log("Failed to retrieve list of characters");
        }


    }

    private void BtnSwitchCharacter(string charID)
    {
        characterToViewID = charID;
        GoToScene(SceneName.ViewCharacter);
    }
}
