using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private TMPro.TextMeshProUGUI UIGameMode; // Skal være synlig på tvers av alle GameModes

    // Start på PlayMode UI elementer
    private GameObject[] UIPlayModeBackgrounds; // Bakgrunnene til target og current player, skrus av når PlayMode ikke er aktiv
    private TMPro.TextMeshProUGUI UIActorName;
    private TMPro.TextMeshProUGUI UIActorHealth;
    private TMPro.TextMeshProUGUI UIActorReach;

    private TMPro.TextMeshProUGUI UITargetName;
    private TMPro.TextMeshProUGUI UITargetHealth;
    private Slider UITargetHealthSlider;
    private TMPro.TextMeshProUGUI UITargetDistance;

    private PlayerController TargetActorController;
    // Slutt på PlayMode UI elementer

    // Start på CreateActorMode UI elementer
    private GameObject[] UIActorProfiles;
    private GameObject UICreateNewActorBackground; // Root UI element for alt som har med å lage nye actors
    private GameObject UIActorListGrid; // Holder på alle ActorProfiles

    public GameObject UIActorProfilePrefab; // Reference to the CreateActorProfile Prefab.
    public GameObject[] UIActorPrefab; // Reference to the Prefab for the different classes.
    public Sprite[] UIActorProfileImage; // De ulike profilbildene. Byttes ut basert på hvilken klasse som velges
    // Slutt på CreateActorMode UI elementer

    // Start på PlaceActorMode UI elementer
    private GameObject UIPlaceActorBackground; // Root UI element for alt som har med å plasser actors på kartet
    public GameObject UIPlaceActorProfilePrefab; // Reference to the PlaceActorProfile Prefab
    private List<GameObject> UIExistingPlaceActorProfiles = new List<GameObject>(); // Henviser til alle profilene som allerede er laget, for å unngå duplikater
    // Slutt på PlaceActorMode UI elementer

    // Start på PauseMenu og SettingsMenu UI elementer
    public GameObject UIPauseMenu;
    public TMPro.TMP_Dropdown UIChooseModeDropdown;
    public TMPro.TMP_Dropdown UIChooseStageDropdown;
    public GameObject UISettingsMenu;
    public TMPro.TextMeshProUGUI UIZoomSensitivityValue;
    public TMPro.TextMeshProUGUI UICameraMovementValue;
    // Slutt på PauseMenu og SettingsMenu UI elementer


    // Start is called before the first frame update
    void Awake()
    {
        UIGameMode = GameObject.FindGameObjectWithTag("UIGameMode").GetComponent<TMPro.TextMeshProUGUI>();

        SetupCreateActorModeUI();
        SetupPlaceActorModeUI();
        SetupGameModeUI();
    }

    private void SetupPlaceActorModeUI()
    {
        UIPlaceActorBackground = GameObject.FindGameObjectWithTag("UIPlaceActor");
        EnablePlaceActorModeUI(false);
    }

    private void SetupCreateActorModeUI()
    {
        
        UICreateNewActorBackground = GameObject.FindGameObjectWithTag("UICreateNewActorProfile");
        UIActorListGrid = GameObject.FindGameObjectWithTag("UIActorList");
    }

    private void SetupGameModeUI()
    {

        UIPlayModeBackgrounds = GameObject.FindGameObjectsWithTag("UIPlayMode");

        UIActorName = GameObject.FindGameObjectWithTag("UIActorName").GetComponent<TMPro.TextMeshProUGUI>();
        UIActorHealth = GameObject.FindGameObjectWithTag("UIActorHealth").GetComponent<TMPro.TextMeshProUGUI>();
        UIActorReach = GameObject.FindGameObjectWithTag("UIActorReach").GetComponent<TMPro.TextMeshProUGUI>();

        UITargetName = GameObject.FindGameObjectWithTag("UITargetName").GetComponent<TMPro.TextMeshProUGUI>();
        UITargetHealth = GameObject.FindGameObjectWithTag("UITargetHealth").GetComponent<TMPro.TextMeshProUGUI>();
        UITargetHealthSlider = GameObject.FindGameObjectWithTag("UITargetSlider").GetComponent<Slider>();
        UITargetHealthSlider.onValueChanged.AddListener(EditTargetHealth); // Sørger for at slideren påvirker livet til target
        UITargetDistance = GameObject.FindGameObjectWithTag("UITargetDistance").GetComponent<TMPro.TextMeshProUGUI>();

        EnablePlayModeUI(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUIActorName(PlayerController nextActor)
    {
        UIActorName.text = "Actor Turn: " + nextActor.ActorName;
    }

    public void SetUIActorHealth(PlayerController nextActor)
    {
        UIActorHealth.text = "Actor Health: " + nextActor.ActorHealth + "/" + nextActor.ActorMaxHealth;
    }

    public void SetUIActorReach(float reach)
    {
        UIActorReach.text = "Reach: " + Mathf.Round(reach);
    }

    public void SetUITargetName(PlayerController targetActor)
    {
        if(TargetActorController != targetActor)
        {
            TargetActorController = targetActor;
        }
        if(targetActor)
        {
            UITargetName.text = "Target: " + targetActor.ActorName;
        } else
        {
            UITargetName.text = "Target: ";
        }
        
    }

    public void SetUITargetHealth(PlayerController targetActor)
    {
        if(targetActor)
        {
            UITargetHealthSlider.enabled = true;
            UITargetHealthSlider.maxValue = targetActor.ActorMaxHealth;
            UITargetHealthSlider.value = targetActor.ActorHealth;
            UITargetHealth.text = UITargetHealthSlider.value.ToString();
        } else
        {
            UITargetHealthSlider.enabled = false;
            UITargetHealth.text = "N/A";
        }
        
    }

    public void SetUITargetDistance(float distance, bool hasTarget = true)
    {
        if(hasTarget)
        {
            UITargetDistance.text = "Distance: " + Mathf.Round(distance);
        } else
        {
            UITargetDistance.text = "Distance: ";
        }
        
    }

    public void EditTargetHealth(float newHealth)
    {
        if(TargetActorController)
        {
            UITargetHealth.text = newHealth.ToString();
            TargetActorController.ActorHealth = (int)Mathf.Round(newHealth);

            if (newHealth == 0)
            {
                UITargetName.text = "Target: " + TargetActorController.ActorName + " (Dead)";
            }
        }
    }

    public void CreateNewActorProfile()
    {
        GameObject newProfile = Instantiate(UIActorProfilePrefab, UIActorListGrid.transform);
        TMPro.TMP_Dropdown dropdown = newProfile.GetComponentInChildren<TMPro.TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(SetActorProfileImage);
    }

    public void CreateNewActors()
    {
        UIActorProfiles = GameObject.FindGameObjectsWithTag("UIActorProfile");
        Vector3 pos = new Vector3(28, 2.09f, 35.12f);
        for (int i = 0; i < UIActorProfiles.Length; ++i)
        {
            
            string actorName = "none";
            int actorInit = 0;
            int actorHealth = 2;
            float actorReach = 0;

            TMPro.TMP_InputField[] inputFields = UIActorProfiles[i].GetComponentsInChildren<TMPro.TMP_InputField>();
            for (int j = 0; j < inputFields.Length; ++j)
            {
                if (inputFields[j].tag == "UIActorNameInput")
                {
                    actorName = inputFields[j].text;
                }
                else if (inputFields[j].gameObject.tag == "UIActorHealthInput")
                {
                    actorHealth = int.Parse(inputFields[j].text);
                }
                else if (inputFields[j].tag == "UIActorInitInput")
                {
                    actorInit = int.Parse(inputFields[j].text);
                }
                else if (inputFields[j].tag == "UIActorSpeedInput")
                {
                    actorReach = float.Parse(inputFields[j].text);
                }
            }

            if (UIActorProfiles[i].GetComponent<ActorProfileManager>().ConnectedActor)
            {
                // Oppdaterer den eksisterende actoren, istedenfor å lage en ny en
                UIActorProfiles[i].GetComponent<ActorProfileManager>().ConnectedActor.
                    GetComponent<PlayerController>().InstantiateActor(actorName, actorHealth, actorInit, actorReach);
            }
            else
            {
                // Lager en ny actor
                pos.x += 5; // Setter posisjonen til denne actoren ved siden av den forrige som ble laget

                /* NB! For at denne indeksen skal bli riktig er det viktig at rekkefølgen 
                * i dropdown menyen stemmer overens med rekkefølgen
                * i arrayet UIActorPrefab. Om ikke så vil feil prefab bli spawnet
                */
                int indexOfPrefabToSpawn = UIActorProfiles[i].GetComponentInChildren<TMPro.TMP_Dropdown>().value;
                GameObject newActor = Instantiate(UIActorPrefab[indexOfPrefabToSpawn], pos, new Quaternion(0, 0, 0, 0));
                newActor.GetComponent<PlayerController>().InstantiateActor(actorName, actorHealth, actorInit, actorReach);
                UIActorProfiles[i].GetComponent<ActorProfileManager>().ConnectedActor = newActor;
            }

        }

        StartPlaceActorModeUI();
    }

    public void StartCreateActorModeUI()
    {
        EnablePlayModeUI(false);
        UICreateNewActorBackground.SetActive(true);
        EnablePlaceActorModeUI(false);
        UIGameMode.text = "Game Mode: Create Actor";
    }

    public void StartPlaceActorModeUI()
    {
        EnablePlayModeUI(false);
        UICreateNewActorBackground.SetActive(false);
        EnablePlaceActorModeUI(true);
        UIGameMode.text = "Game Mode: Place Actor";
    }

    public void StartPlayModeUI()
    {
        EnableCreateActorModeUI(false);
        EnablePlaceActorModeUI(false); 
        EnablePlayModeUI(true);
        UIGameMode.text = "Game Mode: Play";

        SetUITargetDistance(0, false);
        SetUITargetName(null);
        SetUITargetHealth(null);
    }

    public void TogglePauseModeUI(bool enable, int mode)
    {
        UIPauseMenu.SetActive(enable);

        if(enable)
        {
            UIChooseModeDropdown.value = mode;
        }

        if(UISettingsMenu.activeInHierarchy)
        {
            ToggleSettingsMenu();
        }

    }

    private void EnableCreateActorModeUI(bool enable)
    {
        UICreateNewActorBackground.SetActive(false);
    }

    // Skrur av eller på UI elementene for å plassere objekter
    private void EnablePlaceActorModeUI(bool enable)
    {
        UIPlaceActorBackground.SetActive(enable);
    }

    // Skrur av eller på alle UI elementer som hører til PlayMode
    private void EnablePlayModeUI(bool enable)
    {
        for (int i = 0; i < UIPlayModeBackgrounds.Length; ++i)
        {
            UIPlayModeBackgrounds[i].SetActive(enable);
        }
    }

    // Når en profil får en ny klasse, så oppdateres profilbilde på alle profilene
    // Er ikke optimalt, men det funker
    /* NB! For at denne skal fungere er det viktig at rekkefølgen i dropdown menyen stemmer overens med rekkefølgen
       i arrayet UIActorProfileImage. Om ikke så vil feil profilbilde bli satt
    */
    public void SetActorProfileImage(int value)
    {
        UIActorProfiles = GameObject.FindGameObjectsWithTag("UIActorProfile");
        for(int i = 0; i < UIActorProfiles.Length; ++i)
        {
            TMPro.TMP_Dropdown classDropdown = UIActorProfiles[i].GetComponentInChildren<TMPro.TMP_Dropdown>();
            Image[] imgChilds = UIActorProfiles[i].GetComponentsInChildren<Image>();
            for (int j = 0; j < imgChilds.Length; ++j)
            {
                if (imgChilds[j].gameObject.tag == "UIActorImage")
                {
                    imgChilds[j].sprite = UIActorProfileImage[classDropdown.value];
                }
            }
        }
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PopulatePlaceActorOutliner(GameObject[] actors)
    {
        // TODO: Outlineren til PlaceActor mode må fylles opp med et kort for hver actor som viser hvilken som er valgt og 
        // all nødvendig informasjon om Actoren. Det skal ikke gå an å redigere actoren, kun velge en fra lista som skal 
        // flyttes på i scena. Dersom en velges direkte fra scenen så skal også outlineren oppdateres

        for(int i = 0; i < actors.Length; ++i)
        {
            bool makeActor = true;
            foreach(GameObject item in UIExistingPlaceActorProfiles)
            {
                if(item.GetComponent<ActorProfileManager>().ConnectedActor == actors[i])
                {
                    makeActor = false;
                }
            }
            if(makeActor)
            {
                GameObject newProfile = Instantiate(UIPlaceActorProfilePrefab, UIPlaceActorBackground.GetComponentInChildren<VerticalLayoutGroup>().transform);
                newProfile.GetComponent<ActorProfileManager>().InstantiatePlaceActorProfile(actors[i]);
                UIExistingPlaceActorProfiles.Add(newProfile);
            }
        }
    }

    // Deselecter alle profilene i PlaceActorMode. Etterfølges av et SelectProfile()-kall i MouseManager
    public GameObject FindConnectedProfile(GameObject selectedActor)
    {
        foreach (GameObject item in UIExistingPlaceActorProfiles)
        {
            if(item.GetComponent<ActorProfileManager>().ConnectedActor == selectedActor)
            {
                item.GetComponent<ActorProfileManager>().SelectProfile();
                return item;
            } else
            {
                item.GetComponent<ActorProfileManager>().DeselectProfile();
            }
            
        }
        return null;
    }

    public void ToggleSettingsMenu()
    {
        if(UISettingsMenu.activeInHierarchy)
        {
            UISettingsMenu.SetActive(false);
        } else
        {
            UISettingsMenu.SetActive(true);
        }
    }
    public void SetUIZoomSensitivityValue(float newValue)
    {
        UIZoomSensitivityValue.text = roundFloatWithDecimals(newValue).ToString();
    }
    public void SetUICameraMovementValue(float newValue)
    {
        UICameraMovementValue.text = roundFloatWithDecimals(newValue).ToString();
    }
    private float roundFloatWithDecimals(float value)
    {
        value *= 10;
        value = Mathf.Round(value);
        value /= 10;
        return value;
    }
}
