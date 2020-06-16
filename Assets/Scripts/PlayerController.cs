using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public int ActorHealth = 8;
    public int ActorMaxHealth = 8;
    public float ActorDistance = 30;
    public float ActorMaxDistance = 30;
    public string ActorName = "NoName";

    private TurnManager TurnManager;
    private UIManager UIManager;
    private MouseManager MouseManager;

    private Animator anim;
    private NavMeshAgent agent;
    public ParticleSystem ActivePlayerHighlighter;
    public GameObject TorchObject;

    public int ClassID; // Brukes til å finne riktig prefab når man skal lage en ny Actor


    private Vector3 PreviousPos;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        TurnManager = GameObject.FindGameObjectWithTag("TurnManager").GetComponent<TurnManager>();
        UIManager = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UIManager>();
        MouseManager = GameObject.FindGameObjectWithTag("MouseManager").GetComponent<MouseManager>();

        ActorHealth = ActorMaxHealth; // Gir ActorHealth maksverdien av det livet karakteren har
        ResetActorDistance();
        SetPreviousPos(); // Setter PreviousPos til den posisjonen aktøren spawner på
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
            if (MouseManager.targetToPlace == gameObject)
            {
                MouseManager.targetToPlace = null;
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

    public void InstantiateActor(string name, int health, float reach)
    {
        ActorName = name;
        ActorMaxHealth = health;
        ActorHealth = health;
        ActorMaxDistance = reach;
        ActorDistance = reach;
    }

    // Flytter på Actoren til en ny posisjon, samtidig som den oppdaterer PreviousPos
    public void SafelyTransportActor(Vector3 newPos)
    {
        gameObject.transform.position = newPos;
        PreviousPos = newPos;
        GetComponent<NavMeshAgent>().destination = newPos;
    }
}
