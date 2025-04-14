using UnityEngine;

//this script manages the text mesh pro element for ggem total displayed on screen
public class GemTotalIGUI : MonoBehaviour
{
    //get textmeshpro component
    public TMPro.TextMeshProUGUI gemTotalText;
    public GameObject GC;
    public GemCounter playerGemCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get player gem counter component
        playerGemCounter = GC.GetComponent<GemCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        //set texts
        gemTotalText.text = "Gems: " + playerGemCounter.GetGemCount().ToString();
    }
}
