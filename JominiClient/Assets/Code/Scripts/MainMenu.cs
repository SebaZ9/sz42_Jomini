using ProtoMessageClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Controller
{
    public Button btnFiefDetails;
    public Button btnMap;

    public Text lblActiveCharacter;

    public Dropdown ddOwnedCharacters;

    // Start is called before the first frame update
    public void Start()
    {
        btnFiefDetails.onClick.AddListener(MoveToSceneFiefDetails);
        btnMap.onClick.AddListener(() => { GoToScene(SceneName.Map); });

        DisplayCharName();

        ddOwnedCharacters.ClearOptions();
        ProtoGenericArray<ProtoCharacterOverview> ownedCharList = GetNPCList("Family Employ", tclient);
        foreach(ProtoCharacterOverview character in ownedCharList.fields) {
            ddOwnedCharacters.options.Add(new Dropdown.OptionData(character.charID));
        }


        //currentFief = GetFiefDetails(protoClient.activeChar.location, tclient);
        //int index = ddOwnedCharacters.options.FindIndex((i) => { return i.text.Equals(activeCharName); });
        //ddOwnedCharacters.value = index;
        //DisplayFiefDetails();

        ddOwnedCharacters.onValueChanged.AddListener(ChangeCharacter);

        //btnTest.onClick.AddListener(TestFunc);

        //ddOwnedFiefs.ClearOptions();
        //ddOwnedFiefs.AddOptions(new List<string>{"test","that","this","works"});

        //ProtoGenericArray<ProtoFief> ownedFiefsLst = ViewMyFiefs(tclient);
        //foreach(ProtoFief fief in ownedFiefsLst.fields) {
        //    ddOwnedFiefs.options.Add(new Dropdown.OptionData(fief.fiefID));
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MoveToSceneFiefDetails() {
        GoToScene(SceneName.FiefDetails);
    }

    private void ChangeCharacter(int choice) {
        string charID = ddOwnedCharacters.options[choice].text;
        UseChar(charID, tclient);
        DisplayCharName();
    }

    private void DisplayCharName() {
        string activeCharName = protoClient.activeChar.firstName + " " + protoClient.activeChar.familyName;
        lblActiveCharacter.text = "Active Character: \n" + activeCharName;
    }

    //private void TestFunc() {
    //    string myVar = ddOwnedFiefs.options[ddOwnedFiefs.value].text;
    //    ddOwnedFiefs.options.Add(new Dropdown.OptionData(myVar));

        
    //}
}
