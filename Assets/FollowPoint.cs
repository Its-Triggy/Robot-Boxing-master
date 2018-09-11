using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoint : MonoBehaviour {
    public GameObject GoTowards;
    private Rigidbody rb;

    private void Start()
    {
        if (gameObject.activeInHierarchy)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(GoTowards.transform.position);
        rb.MoveRotation(GoTowards.transform.rotation);
    }
}
