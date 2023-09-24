using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(requiredComponent: typeof(ARRaycastManager), 
    requiredComponent2: typeof(ARPlaneManager))]

public class PlaceObject : MonoBehaviour
{   


    [SerializeField]    

    private GameObject prefab;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake() {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();


    }
    
    private void OnEnable() {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable() {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }
    private void FingerDown(EnhancedTouch.Finger finger) {
        if (finger.index != 0) return;

        if (aRRaycastManager.Raycast(screenPoint: finger.currentTouch.screenPosition, hitResults: hits,
            trackableTypes: TrackableType.PlaneWithinPolygon)) {
            foreach(ARRaycastHit hit in hits) {
                Pose pose = hit.pose;
                GameObject obj = Instantiate(original: prefab, position: pose.
                        position, rotation: pose.rotation);

                if(aRPlaneManager.GetPlane(trackableId: hit.trackableId).alignment == PlaneAlignment.HorizontalUp) {
                    UnityEngine.Vector3 position = obj.transform.position;
                    UnityEngine.Vector3 cameraPosition = Camera.main.transform.position;
                    UnityEngine.Vector3 direction = cameraPosition - position;
                    UnityEngine.Vector3 targetRotationEuler = UnityEngine.Quaternion.LookRotation(direction).eulerAngles;
                    UnityEngine.Vector3 scaledEuler = UnityEngine.Vector3.Scale(targetRotationEuler, obj.transform.up.normalized);
                    UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.Euler(scaledEuler);
                    obj.transform.rotation = obj.transform.rotation * targetRotation;
                }
               
            }

        }
    }
}