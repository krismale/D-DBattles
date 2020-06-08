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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            toggleIsPaused();
        }
        
    }

    public void UpdateActorArray()
    {
        Actors = GameObject.FindGameObjectsWithTag("Actor");
    }

    public void PopulateUI(GameObject playerToMove)
    {
        PlayerController ActorPlayerController = playerToMove.GetComponent<PlayerController>();
        UIManager.SetUIActorName(ActorPlayerController);
        UIManager.SetUIActorHealth(ActorPlayerController);
        UIManager.SetUIActorReach(ActorPlayerController.ActorDistance);
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
        } else if(mode == 1)
        {
            
            CreateActorMode = false;
            PlaceActorMode = true;
            UIManager.StartPlaceActorModeUI();
            UpdateActorArray(); // Oppdaterer ActorArray for å være sikker på at vi får med oss riktig antall
            UIManager.PopulatePlaceActorOutliner(Actors); // Legger inn Actors i outlineren til PlaceActorMode
        } else
        {
            CreateActorMode = false;
            PlaceActorMode = false;
            UIManager.StartPlayModeUI();
        }
    }
}
