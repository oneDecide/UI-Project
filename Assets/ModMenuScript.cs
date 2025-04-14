using UnityEngine;

public class ModMenuScript : MonoBehaviour
{
    //get textmeshpro component
    public TMPro.TextMeshProUGUI HPText;
    public TMPro.TextMeshProUGUI SpeedText;
    public TMPro.TextMeshProUGUI ModText;

    public TMPro.TextMeshProUGUI AbilityTitleText;
    public TMPro.TextMeshProUGUI AbilityText;

    public GameObject player;

    public EntityHealth playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get player health component
        playerHealth = player.GetComponent<EntityHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        //set texts
        HPText.text = "HP: " + playerHealth.currentHealth.ToString() + "/" + playerHealth.getMaxHP().ToString();
        SpeedText.text = "Speed: " + player.GetComponent<DefaultCharacter>().moveSpeed.ToString();
        ModText.text = "Mod: N/A";
        AbilityTitleText.text = "None";
        AbilityText.text = "The character has no abilities selected";

    }
}
