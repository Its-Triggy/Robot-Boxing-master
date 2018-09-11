using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnemyFist : MonoBehaviour
{

    int count = 0;

    void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.transform.name);
        if (col.transform.GetComponent<PlayerCharacter>() != null)
        {
            Debug.Log(col.transform.name + count);
            col.transform.GetComponent<PlayerCharacter>().Hurt(1);
            count++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
