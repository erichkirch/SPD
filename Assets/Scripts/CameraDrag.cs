using UnityEngine;
 
 public class CameraDrag : MonoBehaviour {
    //  private Vector3 dragOrigin; //Where are we moving?
    //  private Vector3 clickOrigin = Vector3.zero; //Where are we starting?
    //  private Vector3 basePos = Vector3.zero; //Where should the camera be initially?
 
    //  void Update() {
    //      if (Input.GetMouseButton(0)) {
    //          if (clickOrigin == Vector3.zero) {
    //              clickOrigin = Input.mousePosition;
    //              basePos = transform.position;
    //          }
    //          dragOrigin = Input.mousePosition;
    //      }
 
    //      if (!Input.GetMouseButton(0)) {
    //          clickOrigin = Vector3.zero;
    //          return;
    //      }
 
    //      transform.position = new Vector3(basePos.x + ((clickOrigin.x - dragOrigin.x) * .01f), basePos.y + ((clickOrigin.y - dragOrigin.y) * .01f), -10);
    //  }

    public float perspectiveZoomSpeed = 0.0005f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.0005f;        // The rate of change of the orthographic size in orthographic mode.

    void Update()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (transform.GetComponent<Camera>().orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                transform.GetComponent<Camera>().orthographicSize += (deltaMagnitudeDiff * orthoZoomSpeed) / 5f;

                // Make sure the orthographic size never drops below zero.
                transform.GetComponent<Camera>().orthographicSize = Mathf.Max(transform.GetComponent<Camera>().orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                transform.GetComponent<Camera>().fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                transform.GetComponent<Camera>().fieldOfView = Mathf.Clamp(transform.GetComponent<Camera>().fieldOfView, 0.1f, 179.9f);
            }
        } else if(Input.mouseScrollDelta != Vector2.zero) {
            if (transform.GetComponent<Camera>().orthographic) {
                transform.GetComponent<Camera>().orthographicSize -= Input.mouseScrollDelta.y * orthoZoomSpeed;
                transform.GetComponent<Camera>().orthographicSize = Mathf.Max(transform.GetComponent<Camera>().orthographicSize, 0.1f);
            } else {
                // Otherwise change the field of view based on the change in distance between the touches.
                transform.GetComponent<Camera>().fieldOfView -= Input.mouseScrollDelta.y * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                transform.GetComponent<Camera>().fieldOfView = Mathf.Clamp(transform.GetComponent<Camera>().fieldOfView, 0.1f, 179.9f);
            }
        }
    }
 }