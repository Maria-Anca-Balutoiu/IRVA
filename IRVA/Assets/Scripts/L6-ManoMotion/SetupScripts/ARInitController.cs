using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;

namespace AR_ManoMotion
{
    public class ARInitController : MonoBehaviour
    {
        [Tooltip("Set the device to play on (desktop or AR)")]
        public Globals.DeviceMode deviceMode { get; private set; }

        [Tooltip("Enable or disable ARFoundation's plane detection")]
        public bool enablePlaneDetection = true;

        [Tooltip("Tracking smoothing value")]
        [Range(0f, 1f)] public float trackingSmoothing = 0.5f;

        [Tooltip("AR Session")]
        public ARSession ARSession;

        [Tooltip("AR Session Origin")]
        public ARSessionOrigin ARSessionOrigin;

        [Tooltip("This prebaf will be instantiated in phase 2")]
        public GameObject ARScenePrefab;

        [Header("Phase 3 - Scene adjustments")]
        [Tooltip("Slider used to adjust the instantiated scene's rotation")]
        public Slider sliderRotation;

        [Tooltip("Slider used to adjust the instantiated scene's scale")]
        public Slider sliderScale;

        [Tooltip("Text used to show rotation value (angles)")]
        public TextMeshProUGUI textRotation;

        [Tooltip("Text used to show scale value")]
        public TextMeshProUGUI textScale;

        private ARPlaneManager ARPlaneManager;
        private ARRaycastManager ARRaycastManager;
        private ARSetupPhasesController arSetupPhasesController;
        private bool sceneInstantiated = false;
        private GameObject ARSceneInst = null;
        private Vector3 currentScale = Vector3.one;
        private Quaternion currentRot = Quaternion.identity;

        private void Awake()
        {
            InitDeviceMode();
        }

        void Start()
        {
            // Get required components
            ARPlaneManager = ARSessionOrigin.GetComponent<ARPlaneManager>();
            ARRaycastManager = ARSessionOrigin.GetComponent<ARRaycastManager>();
            arSetupPhasesController = GetComponent<ARSetupPhasesController>();

            // Enable or disable ARFoundation's plane detection
            ARPlaneManager.requestedDetectionMode = enablePlaneDetection ? PlaneDetectionMode.Horizontal : PlaneDetectionMode.None;

            // Set amount of hand motion tracking smoothing
            ManomotionManager.Instance.SetManoMotionSmoothingValueFloat(trackingSmoothing);

            // Setup other vars
            textRotation.text = sliderRotation.value.ToString("0");
            textScale.text = sliderScale.value.ToString("0.00");
        }

        private void Update()
        {
            // If no scene has been instantiated and we're in the appropriate phase
            if (!sceneInstantiated && arSetupPhasesController.CurrentGamePhase == Globals.ARInitPhase.ScenePlacement)
            {
                InstatiateARGameObjects();
            }
        }

        private void InitDeviceMode()
        {
            deviceMode = Application.isMobilePlatform ? Globals.DeviceMode.AR : Globals.DeviceMode.Desktop;
            Globals.CurrentDeviceMode = deviceMode;
        }

        /** PHASE 1 - Confirm button action */
        public void ConfirmPlaneDetection()
        {
            DisablePlaneDetection();
            // Advance from phase 1 to phase 2
            arSetupPhasesController.AdvanceToPhase(Globals.ARInitPhase.ScenePlacement);
        }

        /** PHASE 1 - Reset button action */
        public void ResetPlaneDetection()
        {
            HideDetectedPlanes();
            // Restart the AR session
            ARSession.Reset();
        }

        /** PHASE 2 - Confirm button action */
        public void ConfirmScenePlacement()
        {
            // Advance from phase 2 to phase 3
            arSetupPhasesController.AdvanceToPhase(Globals.ARInitPhase.SceneAdjustments);
            HideDetectedPlanes();
        }

        /** PHASE 2 - Reset button action */
        public void ResetScenePlacement()
        {
            if (ARSceneInst != null)
                Destroy(ARSceneInst);
            sceneInstantiated = false;
        }

        /** PHASE 3 - Rotation slider change action */
        public void SliderRotationChange()
        {
            if (ARSceneInst != null)
            {
                float sliderValue = sliderRotation.value;
                textRotation.text = sliderValue.ToString("0");

                currentRot = Quaternion.Euler(0, sliderValue * -1, 0);  // Note: * -1
                ARSceneInst.transform.rotation = currentRot;

                ARSessionOrigin.MakeContentAppearAt(
                    ARSceneInst.transform,
                    ARSceneInst.transform.position,
                    ARSceneInst.transform.rotation);
            }
        }

        /** PHASE 3 - Scale slider change action */
        public void SliderScaleChange()
        {
            if (ARSceneInst != null)
            {
                float sliderValue = sliderScale.value;
                textScale.text = sliderValue.ToString("0.00");

                currentScale = Vector3.one * sliderValue;
                ARSceneInst.transform.localScale = currentScale;

                ARSessionOrigin.MakeContentAppearAt(
                    ARSceneInst.transform,
                    ARSceneInst.transform.position,
                    ARSceneInst.transform.rotation);
            }
        }

        /** PHASE 3 - Confirm button action */
        public void ConfirmSceneAdjustments()
        {
            // Advance from phase 3 to phase 4
            arSetupPhasesController.AdvanceToPhase(Globals.ARInitPhase.Done);

            if (ARSceneInst != null)
            {
                // Enable game controller scripts
                ARSceneInst.GetComponent<CursorPositionController>().enabled = true;
                ARSceneInst.GetComponent<HandGestureController>().enabled = true;
                ARSceneInst.GetComponentInChildren<FruitSpawner>().enabled = true;
            }
        }

        public void DisablePlaneDetection()
        {
            ARPlaneManager.requestedDetectionMode = PlaneDetectionMode.None;
        }

        public void HideDetectedPlanes()
        {
            foreach (ARPlane plane in ARPlaneManager.trackables)
                plane.gameObject.SetActive(false);
        }

        private void InstatiateARGameObjects()
        {
            // Get hand and gesture information -> used to get trigger-type gesture
            HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;
            GestureInfo gestureInfo = handInfo.gesture_info;
            ManoGestureTrigger manoGestureTrigger = gestureInfo.mano_gesture_trigger;

            // Check if a grab gesture -> if true, instantiate scene
            if (manoGestureTrigger == ManoGestureTrigger.GRAB_GESTURE)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                float screenCoordX = handInfo.tracking_info.palm_center.x * Screen.width;
                float screenCoordY = handInfo.tracking_info.palm_center.y * Screen.height;

                // Ray from camera (based on screen coords) towards world
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenCoordX, screenCoordY, 0));
                if (ARRaycastManager.Raycast(ray, hits))
                {
                    foreach (ARRaycastHit hit in hits)
                    {
                        // Instatiate only once
                        ARSceneInst = Instantiate(ARScenePrefab, hit.pose.position, Quaternion.identity, GameObject.Find("SceneRoot").transform);
                        sceneInstantiated = true;
                        break;
                    }
                    Handheld.Vibrate();
                }
            }
        }
    }
}