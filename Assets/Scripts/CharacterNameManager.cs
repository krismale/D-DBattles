using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNameManager : MonoBehaviour
{
    private TMPro.TextMeshPro UIActorName;
    private RectTransform RectTransform;

    public bool ActorTargeted = false;

    // Start is called before the first frame update
    void Start()
    {
        UIActorName = gameObject.GetComponent<TMPro.TextMeshPro>();
        UIActorName.text = gameObject.GetComponentInParent<PlayerController>().ActorName;
        RectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

        RectTransform.LookAt(Camera.main.transform);
        Quaternion rotation = new Quaternion(transform.rotation.x, 0, 0, transform.rotation.w);
        RectTransform.rotation = rotation;
    }
}
