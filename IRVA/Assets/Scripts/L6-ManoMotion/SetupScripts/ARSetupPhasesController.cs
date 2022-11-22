using UnityEngine;

namespace AR_ManoMotion
{
    public class ARSetupPhasesController : MonoBehaviour
    {
        public ARGameInitCanvasController ARGameInitCanvasController;

        // This will be the starting game phase
        private Globals.ARInitPhase currentGamePhase;
        public Globals.ARInitPhase CurrentGamePhase { get => currentGamePhase; set => currentGamePhase = value; }

        void Start()
        {
            currentGamePhase = Globals.ARInitPhase.PlaneDetection;
            ARGameInitCanvasController.SetEnabledStatesForAll(
                planeDetectionPhase: true,
                scenePlacementState: false,
                sceneConfigsState: false);
        }

        public void AdvanceToPhase(Globals.ARInitPhase newPhase)
        {
            currentGamePhase = newPhase;

            switch (currentGamePhase)
            {
                /** PHASE 1 - Plane detection phase, activate corresponding UI elements */
                case Globals.ARInitPhase.PlaneDetection:
                    ARGameInitCanvasController.SetEnabledStatesForAll(
                        planeDetectionPhase: true,
                        scenePlacementState: false,
                        sceneConfigsState: false); break;

                /** PHASE 2 - Scene placement phase, activate corresponding UI elements */
                case Globals.ARInitPhase.ScenePlacement:
                    ARGameInitCanvasController.SetEnabledStatesForAll(
                        planeDetectionPhase: false,
                        scenePlacementState: true,
                        sceneConfigsState: false); break;

                /** PHASE 3 - Scene adjustments phase, activate corresponding UI elements */
                case Globals.ARInitPhase.SceneAdjustments:
                    ARGameInitCanvasController.SetEnabledStatesForAll(
                        planeDetectionPhase: false,
                        scenePlacementState: false,
                        sceneConfigsState: true); break;

                /** PHASE 3 - AR Init done, game can now start */
                case Globals.ARInitPhase.Done:
                    ARGameInitCanvasController.SetEnabledStatesForAll(
                        planeDetectionPhase: false,
                        scenePlacementState: false,
                        sceneConfigsState: false);

                    enabled = false;
                    break;

                default: ARGameInitCanvasController.SetEnabledStatesForAll(false, false, false); break;
            }
        }
    }
}