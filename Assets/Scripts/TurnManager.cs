using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class TurnManager : MonoBehaviour
{
    public int PlayerTurn = 0;
    private int HighestInitiative;

    public bool CreateActorMode = true;
    public bool PlaceActorMode = false;
    public bool IsPaused = false;
    private int CurrentStage = 1;

    private UIManager UIManager;

    public GameObject[] Actors;

    // Start is called before the first frame update
    void Start()
    {
        UIManager = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UIManager>();
        UIManager.UIChooseModeDropdown.onValueChanged.AddListener(ChangeMode);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
        {
            toggleIsPaused();
        }
        
        if(!CreateActorMode && !PlaceActorMode)
        {
            EnableEndTurn();
        }
        
    }

    // Skrur av og på knappen som lar man ende runden sin
    private void EnableEndTurn()
    {
        if (UIManager.UIEndTurnButton.IsInteractable() && FindActiveActor().GetComponent<NavMeshAgent>().velocity.magnitude > 0)
        {
            UIManager.SetUIEndTurnActive(false);
        }
        else if (FindActiveActor().GetComponent<NavMeshAgent>().velocity.magnitude == 0)
        {
            UIManager.SetUIEndTurnActive(true);
        }
    }

    // Finner antall aktører i scenen
    public int FindHighestInitiative()
    {
        UpdateActorArray();

        for (int i = 0; i < Actors.Length; ++i)
        {
            if (Actors[i].GetComponent<PlayerController>().Initiative > HighestInitiative)
            {
                HighestInitiative = Actors[i].GetComponent<PlayerController>().Initiative;
            }
        }

        return HighestInitiative;
    }

    public void UpdateActorArray()
    {
        List<GameObject> tempAllActors = new List<GameObject>();
        for (int i = 0; i < Resources.FindObjectsOfTypeAll<GameObject>().Length; ++i)
        {
            if (Resources.FindObjectsOfTypeAll<GameObject>()[i].tag == "Actor" && Resources.FindObjectsOfTypeAll<GameObject>()[i].scene == SceneManager.GetActiveScene())
            {

                tempAllActors.Add(Resources.FindObjectsOfTypeAll<GameObject>()[i]);
            }
        }
        Actors = tempAllActors.ToArray(); // Prøver å override Actors til å kun ha de som eksisterer i scenen nå
    }

    // Bytter hvem spiller det er sin tur
    public void IncrementPlayerTurn()
    {
        FindHighestInitiative();
        if(FindActiveActor())
        {
            FindActiveActor().GetComponent<PlayerController>().DeactivateActor();
        }
        

        if (PlayerTurn >= HighestInitiative)
        {
            PlayerTurn = 0;
            while (!DoesInitiativeExist())
            {
                ++PlayerTurn;
            }
        }
        else
        {
            ++PlayerTurn;
            while (!DoesInitiativeExist())
            {
                if(PlayerTurn >= HighestInitiative)
                {
                    PlayerTurn = 0;
                } else
                {
                    ++PlayerTurn;
                }
                
            }
        }

        FindActiveActor().GetComponent<PlayerController>().ActivateActor();
        PopulateUI();
    }

    // Sjekker om det finnes en aktør med Initiative == PlayerTurn. Stemmer dette ikke så vil den øke PlayerTurn
    private bool DoesInitiativeExist()
    {
        for (int i = 0; i < Actors.Length; ++i)
        {
            if(Actors[i].GetComponent<PlayerController>().Initiative == PlayerTurn && Actors[i].activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    private void PopulateUI()
    {
        PlayerController ActorPlayerController = FindActiveActor().GetComponent<PlayerController>();
        UIManager.SetUIActorName(ActorPlayerController);
        UIManager.SetUIActorHealth(ActorPlayerController);
        UIManager.SetUIActorReach(ActorPlayerController.ActorDistance);
    }

    // Finner en aktør som skal flyttes, basert på Initiative og hva PlayerTurn er satt til i TurnManager
    public GameObject FindActiveActor()
    {
        for (int i = 0; i < Actors.Length; ++i)
        {
            if (Actors[i].GetComponent<PlayerController>().Initiative == PlayerTurn)
            {
                return Actors[i]; // Returnerer aktøren som skal flyttes
            }
        }
        return null; // Returnerer null verdi dersom den ikke finner noen med riktig Initiative
    }

    // Setter ny mode vi er i
    // Tillater MouseManager å utføre logikk relatert til PlaceActorMode
    public void MoveToPlaceMode()
    {
        CreateActorMode = false;
        PlaceActorMode = true;
        UpdateActorArray(); // Oppdaterer ActorArray for å være sikker på at vi får med oss riktig antall
        UIManager.PopulatePlaceActorOutliner(Actors); // Legger inn Actors i outlineren til PlaceActorMode
    }

    // Setter ny mode vi er i
    // Tillater MouseManager å utføre logikk relatert til PlayMode
    public void MoveToPlayMode()
    {
        PlaceActorMode = false;
        ChangeStage(CurrentStage - 1);
        if(!DoesInitiativeExist())
        {
            IncrementPlayerTurn();
        }
    }

    public void toggleIsPaused()
    { 
        IsPaused = !IsPaused;

        int mode;
        if(CreateActorMode)
        {
            mode = 0;
        } else if(PlaceActorMode)
        {
            mode = 1;
        } else
        {
            mode = 2;
        }
        UIManager.TogglePauseModeUI(IsPaused, mode);
    }

    public void ChangeMode(int mode)
    {
        
        if(mode == 0)
        {
            CreateActorMode = true;
            PlaceActorMode = false;
            UIManager.StartCreateActorModeUI();
            ActivateAllActors(true);
        } else if(mode == 1)
        {
            
            CreateActorMode = false;
            PlaceActorMode = true;
            UIManager.StartPlaceActorModeUI();
            UpdateActorArray(); // Oppdaterer ActorArray for å være sikker på at vi får med oss riktig antall
            UIManager.PopulatePlaceActorOutliner(Actors); // Legger inn Actors i outlineren til PlaceActorMode
            ActivateAllActors(true);
        } else
        {
            CreateActorMode = false;
            PlaceActorMode = false;
            UIManager.StartPlayModeUI();
            ChangeStage(CurrentStage - 1);
            if (!DoesInitiativeExist())
            {
                IncrementPlayerTurn();
            }
        }
    }

    public void ChangeStage(int stage)
    {
        UpdateActorArray();
        CurrentStage = stage + 1;
        for (int i = 0; i < Actors.Length; ++i)
        {
            if(Actors[i].GetComponent<PlayerController>().Stage == stage + 1 || Actors[i].GetComponent<PlayerController>().Stage == 0)
            {
                Actors[i].SetActive(true);
            } else
            {
                if (FindActiveActor() == Actors[i])
                {
                    IncrementPlayerTurn();
                }
                Actors[i].SetActive(false);
            }
        }
    }

    public void ActivateAllActors(bool activate)
    {
        UpdateActorArray();
        for (int i = 0; i < Actors.Length; ++i)
        {
            Actors[i].SetActive(activate);
        }
    }
}
