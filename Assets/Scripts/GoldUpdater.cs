using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldUpdater : MonoBehaviour
{
    public int gold = 0;
    public int startingGold = 100;
    public Text goldText;

    // Start is called before the first frame update
    void Start()
    {
        gold = startingGold;
    }

    // Update is called once per frame
    void Update()
    {
        goldText.text = "Gold: " + gold;
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public bool SubtractGold(int amount)
    {
        if (amount > gold)
        {
            Debug.Log("Not enough gold!");
            return false;
        }
        gold -= amount;
        return true;
    }
}
