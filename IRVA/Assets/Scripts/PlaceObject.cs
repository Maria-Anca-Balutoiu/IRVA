using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Lean.Touch;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    GameObject previousSelection = null;
    GameObject currentSelection = null;
    bool doubleTap = false;
    float doubleTapTime;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR
    /// background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// A prefab to place when a raycast from a user touch hits a plane.
    /// </summary>
    public GameObject prefab;
    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    /* Only the selected object should be able to be scaled, rotated or translated */
    void SelectObject(GameObject selected)
    {
        previousSelection = currentSelection;

        /* Remove translation, rotation and scaling scripts for previously selected object */
        if (currentSelection != null)
        {
            /* TODO 2.2 Destroy DragObject, LeannPinchScale and LeanTwistRotateAxis
             * components for previously selected object
             */
        }

        currentSelection = selected;

        /* TODO 2.2 Add the translation, rotation and scaling scripts to the current objects */

        /* Pinch and twist gestures require two fingers on screen */
        currentSelection.GetComponent<LeanPinchScale>().Use.RequiredFingerCount = 2;
        currentSelection.GetComponent<LeanTwistRotateAxis>().Use.RequiredFingerCount = 2;
    }

    /* Check if the user has tapped on screen. CHeck if an object was selected*/
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            touchPosition = default;
            return false;
        }

        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;

            Ray ray = FirstPersonCamera.ScreenPointToRay(touchPosition);
            RaycastHit hitObject = new RaycastHit();

            /* Check if a 3D object was hit */
            /* TODO 1.1 Check if 3D object was tapped */
            if (true)
            {
                if (hitObject.transform.tag == "Manipulated")
                {
                    if (hitObject.transform.gameObject != currentSelection)
                    {
                        doubleTap = false;
                        doubleTapTime = Time.time;

                        /* TODO 1.2 Call function which adds the scripts for object manipulation */
                    }
                    else
                    {
                        /* Check if a small amount of time has passed since the last tap on the same object */
                        if (Time.time < doubleTapTime + 0.3f)
                        {
                            doubleTap = true;
                        }

                        doubleTapTime = Time.time;
                    }

                    /* If double tap eventa occured, change translation time */
                    if (doubleTap == true)
                    {
                        doubleTap = false;
                        DragObject.elevate = !DragObject.elevate;
                    }

                    touchPosition = default;
                    return false;
                }
            }

            return true;
        }

        touchPosition = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        /* Check if an AR FOundation trackable was hit */
        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            /* Add a new object in scene */
            var hitPose = s_Hits[0].pose;
            spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
            spawnedObject.transform.localScale *= 2.0f;
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    ARRaycastManager m_RaycastManager;

    /* TODO 5 Add a button and a function which delete the currently selected object */
    /* TODO 6 Add a Vuforia AR object which can be manipulated (translation, rotation, scaling) */
}
