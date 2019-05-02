using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    public static Text log;
    public static Text health;
    public static Image healthBar;
    public static Text level;
    public static Text xp;
    public static Image xpBar;

    // Start is called before the first frame update
    void Start()
    {
        log = this.transform.Find("LogText").GetComponent<Text>();
        health = this.transform.Find("EmptyHealth").Find("HealthText").GetComponent<Text>();
        health.text = Player.baseHealth + " / " + Player.baseHealth;
        healthBar = this.transform.Find("EmptyHealth").Find("HealthBar").GetComponent<Image>();
        healthBar.type = Image.Type.Filled;
        healthBar.fillAmount = 1.0f;

        level = this.transform.Find("LevelText").GetComponent<Text>();
        xp = this.transform.Find("EmptyXP").Find("xpText").GetComponent<Text>();
        xpBar = this.transform.Find("EmptyXP").Find("XPBar").GetComponent<Image>();
        xpBar.type = Image.Type.Filled;
        xpBar.fillAmount = 0.0f;
    }

    public static void logPrint(string msg) {
        if(log.text.Length > 390) {
            log.text = "";
        }
        log.text = msg + "\n" + log.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
