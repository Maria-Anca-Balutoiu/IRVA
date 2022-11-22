using UnityEngine;

namespace AR_ManoMotion
{
    public class CursorPositionController : MonoBehaviour
    {
        [Tooltip("Object which will follow the cursor (mouse on desktop or hand position in AR")]
        public GameObject CursorPrefab;

        // Cursor position in screen space
        private Vector3 cursorScreenPos;
        // Main camera reference
        private Camera m_Camera;
        // Precompute
        private float piPer180 = Mathf.PI / 180f;

        void Start()
        {
            if (m_Camera == null)
                m_Camera = Camera.main;
        }

        void Update()
        {
            if (Globals.CurrentDeviceMode == Globals.DeviceMode.Desktop)
            {
                // If playing on computer, the in-game cursor position in the mouse position
                cursorScreenPos = Input.mousePosition;
            }
            else if (Globals.CurrentDeviceMode == Globals.DeviceMode.AR)
            {
                /** L6_TODO:
                 * In this case, we need to set 'cursorScreenPos' to the on-screen hand position detected by ManoMotion -> more precisely, the palm center position
                 * To do this, get the hand information, from which you can access the palm center position
                 * (!) NOTE:
                 *      -> Palm center is returned in normalized coordinates [0...1], 'cursorScreenPos' needs to be in screen coordinates (pixels)
                 *      -> Use screen.width & Screen.height to compute position in pixels
                 *      -> Z coord can be set to 0 */
            }

            UpdateCursorPostion(cursorScreenPos);
        }

        /// <summary>
        /// This keeps the 3D cursor object on an imaginary plane positioned at PlayPlane's position with its normal aligned with PlayPlane's forward vector
        /// Why? We can easily find the cursor positon in screen space, but we need to make a 3D object follow the hand's position in world space.
        /// As we de not have depth info (except some ManoMotion approximation and ignoring LiDAR sensors), we need to position that object somewhere.
        /// This 'somewhere' is in this case on the above-described plane. Take notice that all fruits are spawned on the same plane as well.
        /// We basically remove one movement axis and force everything to happen on the same plane - so it all behaves nicely.
        /// </summary>
        /// <param name="screenPointPos"></param>
        private void UpdateCursorPostion(Vector3 screenPointPos)
        {
            // Create ray through screen position towards world
            Ray cameraRay = m_Camera.ScreenPointToRay(screenPointPos);
            // Compute angle between ray and play scene forward vector
            var playSceneCamAngleRad = Vector3.Angle(transform.forward, cameraRay.direction) * piPer180;
            // Compute camera-play plane distance (z-axis only)
            float camPlaySceneDistZ = Mathf.Abs(m_Camera.transform.position.z - transform.position.z);
            // Position the cursor on a plane 'cameraPlayPlaneDist' distance away using some trig
            Vector3 targetPos = m_Camera.transform.position + (cameraRay.direction * (camPlaySceneDistZ / Mathf.Cos(playSceneCamAngleRad)));

            if (targetPos == Vector3.zero)
                return;

            // Set cursor position
            CursorPrefab.transform.position = targetPos;
        }
    }
}