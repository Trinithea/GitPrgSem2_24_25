using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class CardSelector : MonoBehaviour
{
    public float maxDistance = 80f;
    public float selectCooldown = 0.5f;
    public TextMeshProUGUI scoreText;
    private GameObject markedCard = null;
    
    
    private float cooldown = 0f;
    private int score = 0;
    private Camera camera;

    private void Start()
    {
        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        cooldown += Time.deltaTime;
        Debug.DrawRay(camera.transform.position, camera.transform.forward * maxDistance, Color.red, 5);
        if (cooldown < selectCooldown)
            return;
        if (!Input.GetMouseButton(0))
            return;
        
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        Debug.Log("Cast a ray");
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log($"raycast Hit {hit.collider.name}");
            if (hit.collider.CompareTag("CardTag"))
            {
                Debug.Log("Hit a card");
                cooldown = 0;
                if (hit.collider.gameObject.GetComponent<CardScript>().isFlipped)
                    return;
                
                if (markedCard == null)
                {
                    markedCard = hit.collider.gameObject;
                    Debug.Log("marked card");
                    hit.collider.gameObject.GetComponent<CardScript>().FlipCard();
                    break;
                }
                    
                handleSecondCard(hit.collider.gameObject);
                break;
            }
        }
    }

    private void handleSecondCard(GameObject card)
    {
        if (markedCard == card)
            return;
        card.GetComponent<CardScript>().FlipCard();
        
        int val1 = markedCard.GetComponent<CardScript>().GetValue();
        int val2 = card.GetComponent<CardScript>().GetValue();

        if (val1 == val2)
        {
            Debug.Log("match");
            score++;
            scoreText.text = score.ToString();
                
            markedCard = null;

            if (score == 6 * 4 * 4 / 2)
            {
                this.GetComponent<CardSpawner>().NewGame();
                score = 0;
            }
                
            return;
        }
        
        Debug.Log($"no match, selected {val1} and {val2}");
        StartCoroutine(WaitAndFlip(card, markedCard));
        markedCard = null;
    }

    IEnumerator WaitAndFlip(GameObject card, GameObject _markedCard)
    {
        yield return new WaitForSeconds(1.5f);
        
        _markedCard.GetComponent<CardScript>().FlipCard();
        card.GetComponent<CardScript>().FlipCard();

    }
}
