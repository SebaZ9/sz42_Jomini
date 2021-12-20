using ProtoMessageClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YourFiefs : Controller
{
    public Transform CanvasParent;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start() {        


        ProtoMessage reply = ViewMyFiefs(tclient);
        if (reply.ResponseType == DisplayMessages.Success) {

            fiefList = (ProtoGenericArray<ProtoFief>)reply;
            if (fiefList.fields == null) {
                Debug.Log("No valid fiefs");
            } else {

                foreach (ProtoFief fief in fiefList.fields) {

                    string status;
                    switch (fief.status) {
                        case 'C': status = "Calm"; break;
                        case 'U': status = "Unrest"; break;
                        case 'R': status = "Rebellion"; break;
                        default: status = "Unknown"; break;
                    }

                    GameObject UIElement = Instantiate(prefab);
                    UIElement.transform.SetParent(CanvasParent);

                    UIElement.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText(fief.FiefName);
                    UIElement.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().SetText(fief.population.ToString());
                    UIElement.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().SetText(fief.industry.ToString("F2"));
                    UIElement.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().SetText(fief.fields.ToString("F2"));
                    UIElement.transform.GetChild(4).GetComponent<TMPro.TMP_Text>().SetText(fief.keepLevel.ToString("F2"));
                    UIElement.transform.GetChild(5).GetComponent<TMPro.TMP_Text>().SetText(status);
                    UIElement.transform.GetChild(6).GetComponent<TMPro.TMP_Text>().SetText(fief.treasury.ToString());
                    UIElement.transform.localScale = Vector3.one;

                    UIElement.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(() => { BtnViewFief(fief.fiefID); });

                }

            }                             

        } else {
            Debug.Log("Failed to retrieve list of fiefs");
        }


    }

    private void BtnViewFief(string fiefID) {
        fiefToViewID = fiefID;
        GoToScene(SceneName.ViewFief);
    }
}
