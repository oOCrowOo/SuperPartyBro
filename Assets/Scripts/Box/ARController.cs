using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ARController : MonoBehaviour
{
    //We will fill the list with the planes that ARCore detected in the current frame
    private List<TrackedPlane> m_NewTrackedPlanes = new List<TrackedPlane>();

    public GameObject GridPrefab;

    public GameObject Box;

    public GameObject ARCamera;

    public GameObject PaperPanel;

    private bool EnableBox;

    public GameManager myManager;


    // Start is called before the first frame update
    void Start()
    {
        EnableBox = false;
    }

    // Update is called once per frame
    void Update()
    {
        //check AR session status
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        //The following function will fill m_NewTrackedPlanes with the planes that ARCore detected in the current frame
        Session.GetTrackables<TrackedPlane>(m_NewTrackedPlanes, TrackableQueryFilter.New);

        //Instantiate a Grid for each TrackedPlane in m_NewTrackedPlanes
        for (int i = 0; i < m_NewTrackedPlanes.Count; ++i)
        {
            GameObject grid = Instantiate(GridPrefab, Vector3.zero, Quaternion.identity, transform);

            //This function will set the position of grid and modify the vertices of the attached mesh
            grid.GetComponent<GridVisualiser>().Initialize(m_NewTrackedPlanes[i]);
        }

        //Check if the user touches the screen
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        if(!EnableBox)
        {
            //Check if the user touched any of the tracked planes
            TrackableHit hit;
            if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit))
            {
                //Now place the portal on top of the tracked plane that we touched

                //Enable the portal
                Box.SetActive(true);

                //Create a new Anchor
                Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

                //Set the position pf the portal to be the same as the hit position
                Box.transform.position = hit.Pose.position;
                Box.transform.rotation = hit.Pose.rotation;
                //We want the portal to face the camera
                Vector3 cameraPosition = ARCamera.transform.position;

                //The portal should only rotate the Y axis
                cameraPosition.y = hit.Pose.position.y;

                //Rotate the portal to face the camera
                Box.transform.LookAt(cameraPosition, Box.transform.up);
                Box.transform.Rotate(new Vector3(0, 180, 0));

                //ARCore will keep unstanding the world and update the anchor accordingly hence we need to attach our portal to the anchor
                Box.transform.parent = anchor.transform;
                EnableBox = true;

                // Clicked screen, box shows up, state switch
                PaperPanel.SetActive(true);
                myManager.finishScanning();
            }
        }
        
    }
}
