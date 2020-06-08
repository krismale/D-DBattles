using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{

    //Know what objects are clickable
    public LayerMask clickableLayer;

    //Swap cursors per object
    public Texture2D pointer; // The regular pointer, can't click on anything
    public Texture2D moveToCursor;  // Cursor for clickable objects
    public Texture2D targetCursor;  // Cursor for combat actions
    public Texture2D notAllowedCursor; // Cursor for illegal move

    public EventVector3 OnClickEnvironment;

    public TurnManager TurnManager;
    public UIManager UIManager;

    private Color32 targetFontColor = new Color32(255, 255, 255, 255);
    private Color32 normalFontColor = new Color32(171, 171, 171, 255);

    public GameObject targetToPlace;


    void Start()
    {
        UIManager = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UIManager>();
        OnClickEnvironment.AddListener(ActorMoveTo);
    }

    // Update is called once per frame
    void Update()
    {

        if(EventSystem.current.IsPointerOverGameObject() || TurnManager.IsPaused)
        {
            // Sets the cursor back to normal, if it's hovering over a UI element or the game is paused
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
            return;
        }
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 150, clickableLayer.value))
        {
            if(TurnManager.CreateActorMode)
            {
                // Gjør ingenting, annet enn så sette default cursor
                Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
            } else if(TurnManager.PlaceActorMode)
            {
                PlaceActorLogic(hit);
            }
            else
            {
                ActiveGameLogic(hit);
            }
            
        }
        else
        {
            // Sets the cursor back to normal, if it's neither hovering over or clicking on an object
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
        }
    }

    private void PlaceActorLogic(RaycastHit hit)
    {
        bool actorHit = false;
        if (hit.collider.gameObject.tag == "Actor")
        {
            Cursor.SetCursor(targetCursor, new Vector2(16, 16), CursorMode.Auto);
            actorHit = true;
        }
        else if (targetToPlace)
        {
            Cursor.SetCursor(moveToCursor, new Vector2(16, 16), CursorMode.Auto);
            actorHit = false;
        }
        else
        {
            actorHit = false;
            // Sets the cursor to show that you can click on an object
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
        }
        if (Input.GetMouseButtonDown(1))
        {
            TurnManager.UpdateActorArray();
            if (actorHit)
            {
                for (int i = 0; i < TurnManager.Actors.Length; ++i)
                {
                    targetToPlace = hit.collider.gameObject;
                    if (TurnManager.Actors[i] == targetToPlace)
                    {
                        targetToPlace.GetComponentInChildren<TMPro.TextMeshPro>().color = targetFontColor;
                        UIManager.FindConnectedProfile(targetToPlace);
                    }
                    else
                    {
                        TurnManager.Actors[i].GetComponentInChildren<TMPro.TextMeshPro>().color = normalFontColor;
                    }
                }
            }
            else
            {
                targetToPlace.GetComponentInChildren<TMPro.TextMeshPro>().color = normalFontColor;
                targetToPlace = null;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (targetToPlace)
            {
                targetToPlace.transform.position = hit.point;
            }

        }
    }

    private void ActiveGameLogic(RaycastHit hit)
    {
        bool actorHit = false;
        bool cannotMove = false;
        if (hit.collider.gameObject.tag == "Actor")
        {
            Cursor.SetCursor(targetCursor, new Vector2(16, 16), CursorMode.Auto);
            actorHit = true;
        }
        else if (hit.collider.gameObject.tag == "Wall")
        {
            Cursor.SetCursor(notAllowedCursor, new Vector2(16, 16), CursorMode.Auto);
            cannotMove = true;
        }
        else if (targetToPlace)
        {
            Cursor.SetCursor(moveToCursor, new Vector2(16, 16), CursorMode.Auto);
            actorHit = false;
        }
        else
        {
            actorHit = false;
            // Sets the cursor to show that you can click on an object
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
        }
        if (Input.GetMouseButtonDown(1))
        {
            TurnManager.UpdateActorArray();
            if (actorHit)
            {
                if (targetToPlace)
                {
                    targetToPlace.GetComponent<PlayerController>().DeactivateActor();
                }
                targetToPlace = hit.collider.gameObject;
                for (int i = 0; i < TurnManager.Actors.Length; ++i)
                {
                    if (TurnManager.Actors[i] == targetToPlace)
                    {
                        targetToPlace.GetComponent<PlayerController>().ActivateActor();
                        TurnManager.PopulateUI(targetToPlace);
                    }
                }
            }
            else
            {
                //Kan ikke spilleren bevege seg til det oppgitte punktet så hopper vi tidlig ut av funksjonen her
                if (cannotMove || !targetToPlace)
                {
                    return;
                }
                OnClickEnvironment.Invoke(hit.point);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (actorHit)
            {
                TurnManager.UpdateActorArray();
                for (int i = 0; i < TurnManager.Actors.Length; ++i)
                {
                    if (TurnManager.Actors[i] == hit.collider.gameObject)
                    {
                        TurnManager.Actors[i].GetComponentInChildren<TMPro.TextMeshPro>().color = targetFontColor;
                    }
                    else
                    {
                        TurnManager.Actors[i].GetComponentInChildren<TMPro.TextMeshPro>().color = normalFontColor;
                    }
                }
                float distToActor = 0;
                if (targetToPlace)
                {
                    distToActor = (hit.collider.gameObject.transform.position - targetToPlace.transform.position).magnitude;
                }
                
                UIManager.SetUITargetDistance(distToActor);
                UIManager.SetUITargetName(hit.collider.gameObject.GetComponent<PlayerController>());
                UIManager.SetUITargetHealth(hit.collider.gameObject.GetComponent<PlayerController>());
            }
        }
    }

    void ActorMoveTo(Vector3 MoveTo)
    {
        if((MoveTo - targetToPlace.transform.position).magnitude > targetToPlace.GetComponent<PlayerController>().ActorDistance)
        {
            Debug.Log((MoveTo - targetToPlace.transform.position).magnitude + ": Out of reach");
            return;
        }

        targetToPlace.GetComponent<NavMeshAgent>().destination = MoveTo;
    }

}

[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }
