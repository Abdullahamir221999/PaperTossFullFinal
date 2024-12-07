using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public ShopData shopData;

    public GameObject BallButton;
    public GameObject ScrollerContentPane;

    public UIManager uiManager;
    public TextMeshProUGUI coinText;
    private List<GameObject> instantiatedButtons = new List<GameObject>();

    private void Start()
    {
        // Loop through each shop item in the shop data
        for (int i = 0; i < shopData.shopItems.Length; i++)
        {
            ShopItem item = shopData.shopItems[i];

            // Instantiate a new BallButton and set its parent to the ScrollerContentPane
            GameObject buttonInstance = Instantiate(BallButton, ScrollerContentPane.transform);
            instantiatedButtons.Add(buttonInstance);

            // Update the button UI
            UpdateButtonUI(i, buttonInstance);

            // Add a listener to the button
            Button button = buttonInstance.GetComponent<Button>();
            int buttonID = i; // Capture the current index for the listener
            button.onClick.AddListener(() => ButtonClicked(buttonID, buttonInstance));
        }
    }

    private void UpdateButtonUI(int buttonID, GameObject buttonInstance)
    {
        ShopItem item = shopData.shopItems[buttonID];

        TextMeshProUGUI stateText = buttonInstance.transform.Find("BallState").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI rarityText = buttonInstance.transform.Find("BallRarity").GetComponent<TextMeshProUGUI>();
        Image ballImage = buttonInstance.transform.Find("BallImage").GetComponent<Image>();

        // Set the ball image
        ballImage.sprite = item.ballSprite;

        // Set the state text based on the ball state
        switch (item.ballState)
        {
            case BallState.Bought:
                stateText.text = "";
                break;
            case BallState.Equipped:
                stateText.text = "Equipped";
                break;
            case BallState.NotBought:
                stateText.text = ((int)item.ballType).ToString();
                break;
        }

        // Set the rarity text (price) based on the ball type
        rarityText.text = item.ballType.ToString();
    }

    public void ButtonClicked(int ButtonID, GameObject buttonInstance)
    {
        ShopItem item = shopData.shopItems[ButtonID];

        if (item.ballState == BallState.Equipped)
        {
            Debug.Log("Ball is already equipped. No action taken.");
            return;
        }

        if (item.ballState == BallState.Bought)
        {
            // Update the state of the currently equipped ball to "Bought"
            for (int i = 0; i < shopData.shopItems.Length; i++)
            {
                if (shopData.shopItems[i].ballState == BallState.Equipped)
                {
                    shopData.shopItems[i].ballState = BallState.Bought;
                    UpdateButtonUI(i, instantiatedButtons[i]);
                    break;
                }
            }

            // Set the clicked ball's state to "Equipped"
            item.ballState = BallState.Equipped;
            UpdateButtonUI(ButtonID, buttonInstance);
            Debug.Log("Ball equipped: " + item.ballName);
        }
        else if (item.ballState == BallState.NotBought)
        {
            int playerCoins = PlayerPrefs.GetInt("Coins", 0);
            Debug.Log("Player Coins are: " + playerCoins);
            if (playerCoins >= (int)item.ballType)
            {
                // Deduct the price and update PlayerPrefs
                playerCoins -= (int)item.ballType;
                PlayerPrefs.SetInt("Coins", playerCoins);
                Debug.Log("New Player Coins are: " + PlayerPrefs.GetInt("Coins"));
                uiManager.coinText.text = playerCoins.ToString();
                uiManager.UpdateCoins();
                
                // Update the state of the ball to "Bought"
                item.ballState = BallState.Bought;
                UpdateButtonUI(ButtonID, buttonInstance);
                Debug.Log("Ball bought: " + item.ballName);
            }
            else
            {
                Debug.Log("Not enough coins to buy the ball.");
            }
        }
        
        
    }
}
