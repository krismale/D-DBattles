using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorProfileManager : MonoBehaviour
{
    public GameObject ConnectedActor;
    public bool IsPlaceActorProfile = false;
    public Color SelectedColor = new Color(14, 0, 255, 255);
    public Color DeselectedColor = new Color(65, 65, 65, 255);

    public TMPro.TextMeshProUGUI ActorNameText;
    public TMPro.TextMeshProUGUI ActorClassText;


    // Start is called before the first frame update
    void Update()
    {

    }

    public void InstantiatePlaceActorProfile(GameObject actor)
    {
        // Finner de riktige komponentene
        if (IsPlaceActorProfile)
        {
            for (int i = 0; i < gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>().Length; ++i)
            {
                if (gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[i].tag == "UIActorName")
                {
                    ActorNameText = gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[i];
                }
                else if (gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[i].tag == "UIActorClass")
                {
                    ActorClassText = gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[i];
                }
            }

        }
        ConnectedActor = actor;
        // Setter verdiene til komponentene
        if (IsPlaceActorProfile)
        {
            ActorNameText.text = actor.GetComponent<PlayerController>().ActorName;

            if (actor.GetComponent<PlayerController>().ClassID == 0)
            {
                ActorClassText.text = "Default Class";
            }
            else if (actor.GetComponent<PlayerController>().ClassID == 1)
            {
                ActorClassText.text = "Barbarian";
            }
            else if (actor.GetComponent<PlayerController>().ClassID == 2)
            {
                ActorClassText.text = "Goblin";
            }
            else if (actor.GetComponent<PlayerController>().ClassID == 3)
            {
                ActorClassText.text = "Knight";
            }
            else if (actor.GetComponent<PlayerController>().ClassID == 4)
            {
                ActorClassText.text = "Troll";
            }
            else if (actor.GetComponent<PlayerController>().ClassID == 5)
            {
                ActorClassText.text = "Paladin";
            }
            else
            {
                ActorClassText.text = "NoClass";
            }
        }
    }

    public GameObject SelectProfile()
    {
        if(IsPlaceActorProfile)
        {
            gameObject.GetComponent<Image>().color = SelectedColor;
            return gameObject;
        } else
        {
            return null;
        }
    }

    public void DeselectProfile()
    {
        if (IsPlaceActorProfile)
        {
            gameObject.GetComponent<Image>().color = DeselectedColor;
        }
    }
}
