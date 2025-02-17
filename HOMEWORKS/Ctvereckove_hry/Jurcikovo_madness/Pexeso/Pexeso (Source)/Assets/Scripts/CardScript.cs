using System;
using TMPro;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    public int cardValue;
    public bool isFlipped { get; private set; } = false;
    private TextMeshProUGUI textDisplay;

    private void Awake()
    {
        textDisplay = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetValue(int value)
    {
        cardValue = value;
        textDisplay.text = value.ToString();
    }
    
    public int GetValue()
    {
        return cardValue;
    }

    public void FlipCard()
    {
        isFlipped = !isFlipped;
        transform.localRotation *= Quaternion.Euler(180f, 0f, 0f);
        Debug.Log($"Flipped num {cardValue}");
    }
}
