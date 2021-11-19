using ProtoMessageClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalEntries : Controller {
    public Transform CanvasParent;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start() {

        ProtoMessage reply = ViewJournalEntries("all", tclient);
        if (reply.ResponseType == DisplayMessages.JournalEntries) {
            journalList = (ProtoGenericArray<ProtoJournalEntry>)reply;

            foreach (ProtoJournalEntry journalEntry in journalList.fields) {

                string season = SeasonToString(journalEntry.season);
                string location = string.IsNullOrWhiteSpace(journalEntry.location) ? "" : fiefNames[journalEntry.location];

                GameObject UIElement = Instantiate(prefab);
                UIElement.transform.SetParent(CanvasParent);
                UIElement.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText(journalEntry.jEntryID.ToString());
                UIElement.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().SetText(journalEntry.year.ToString());
                UIElement.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().SetText(season);
                UIElement.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().SetText(journalEntry.type);
                UIElement.transform.GetChild(4).GetComponent<TMPro.TMP_Text>().SetText(location);
                UIElement.transform.localScale = Vector3.one;

                if (viewingListSelectAction.Equals("ArmiesInFief") || viewingListSelectAction.Equals("YourArmies")) {
                    UIElement.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => { BtnViewJournalEntry(journalEntry.jEntryID); });
                }

            }

        } else {
            Debug.Log("Failed to retrieve journal list");
        }


    }

    private void BtnViewJournalEntry(uint jEntryID) {
        journalEntryToViewID = jEntryID;
        GoToScene(SceneName.ViewJournalEntry);
    }
}
