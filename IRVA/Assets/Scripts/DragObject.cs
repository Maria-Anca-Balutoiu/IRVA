using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Touch touch;
    private float speedModifier;
    public static bool elevate;

    public GameObject a;
    // Start is called before the first frame update
    void Start()
    {
        speedModifier = 0.001f;
        elevate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            /* TODO 3 This line moves the object up-down. Change it to move the object left-right and front-back */
            transform.position += new Vector3(0, touch.deltaPosition.y * speedModifier, 0);

            /* TODO 4 Switch between left-right, front-back movement and up-down movement.
             * Hint! Check how elevate variable changes in other scripts. 
             */
        }
    }
}
