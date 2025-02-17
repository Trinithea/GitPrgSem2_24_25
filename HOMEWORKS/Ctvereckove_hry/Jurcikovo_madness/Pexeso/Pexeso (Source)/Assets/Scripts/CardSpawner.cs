using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public int size = 4;
    public float spacing = 1f;
    public Transform[] walls;
    private int[] cardValues;

    void Start()
    {
        generateCardValues();
        assingValues();
    }



    // ReSharper disable Unity.PerformanceAnalysis
    public void NewGame()
    {
        generateCardValues();
        assingValues();
        
        foreach (Transform wall in walls)
        {
            foreach (Transform card in wall)
            {
                CardScript script = card.GetComponent<CardScript>();
                script.FlipCard();
            }
        }
    }
    private void generateCardValues()
    {
        int arrayLen = size * size * 6;
        cardValues = new int[arrayLen];
        for (int i = 0; i < arrayLen ; i += 2)
        {
            cardValues[i] = i;
            cardValues[i + 1] = i;
        }

        System.Random rng = new();
        for (int i = 0; i < arrayLen; i++)
        {
            int randomIndex = rng.Next(0, arrayLen);
            (cardValues[i], cardValues[randomIndex]) = (cardValues[randomIndex], cardValues[i]);
        }
    }

    private void assingValues()
    {
        int cardIndex = 0;
        foreach (Transform wall in walls)
        {
            foreach (Transform card in wall)
            {
                CardScript script = card.GetComponent<CardScript>();
                script.SetValue(cardValues[cardIndex]);
                cardIndex++;
            }
        }
    }
}
