using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public int Initiative = 0;
    public int ActorHealth = 8;
    public int ActorMaxHealth = 8;
    public float ActorDistance = 30;
    public float ActorMaxDistance = 30;
    public string ActorName = "NoName";
    public int Stage = 0;
    

    private Animator anim;
    private NavMeshAgent agent;
    private TurnManager TurnManager;
    private UIManager UIManager;
    private ParticleSystem ActivePlayerHighlighter;

    public int ClassID; // Brukes til å finne riktig prefab når man skal lage en ny Actor


    private Vector3 PreviousPos;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        TurnManager = GameObject.FindGameObjectWithTag("TurnManager").GetComponent<TurnManager>();
        UIManager = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UIManager>();
        ActivePlayerHighlighter = gameObject.GetComponentInChildren<ParticleSystem>();

        ActorHealth = ActorMaxHealth; // Gir ActorHealth maksverdien av det livet karakteren har
        ResetActorDistance();
        SetPreviousPos(); // Setter PreviousPos til den posisjonen aktøren spawner på
    }

    void Start()
    {
        if(Initiative == 0)
        {
            ActivateActor();
        }
    }

    void Update()
    {
        if(anim)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
        
        KillActor();
        UpdateDistance();
    }

    private void KillActor()
    {
        if (ActorHealth <= 0)
        {
            gameObject.tag = "Untagged";
            if (TurnManager.FindActiveActor() == gameObject)
            {
                // Setter turen til neste aktør dersom det er denne aktørens tur
                TurnManager.IncrementPlayerTurn();
            } else
            {
                TurnManager.FindHighestInitiative();
            }
            Destroy(gameObject);
            
        }
    }

    private void SetPreviousPos()
    {
        PreviousPos = gameObject.transform.position;
    }

    private void UpdateDistance()
    {
        if(agent.velocity.magnitude > 0)
        {
            ActorDistance = ActorDistance - (gameObject.transform.position - PreviousPos).magnitude;
            UIManager.SetUIActorReach(ActorDistance);
            SetPreviousPos();
        }
    }

    public void ActivateActor()
    {
        ResetActorDistance();
        ActivePlayerHighlighter.Play();

    }

    public void DeactivateActor()
    {
        ActivePlayerHighlighter.Stop();
    }

    // Gir ActorDistance maksverdien av rekkevidden til karakteren
    private void ResetActorDistance()
    {
        ActorDistance = ActorMaxDistance; 
    }

    public void ActivateActivePlayerHighlighter(bool activate)
    {
        if(activate)
        {
            ActivePlayerHighlighter.Play();
        } else
        {
            ActivePlayerHighlighter.Stop();
        }
    }

    public void InstantiateActor(string name, int health, int initiative, float reach)
    {
        ActorName = name;
        ActorMaxHealth = health;
        ActorHealth = health;
        Initiative = initiative;
        ActorMaxDistance = reach;
        ActorDistance = reach;
    }
}
