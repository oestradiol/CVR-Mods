using System.Collections;
using ABI_RC.Core;
using UnityEngine;
using static ThirdPerson.ThirdPerson;

namespace ThirdPerson;

internal static class CameraLogic
{
    private static float _dist;
    private static GameObject _ourCam, _defaultCam;
    internal static CameraLocation CurrentLocation = CameraLocation.Default;
    internal enum CameraLocation
    {
        Default,
        FrontView,
        RightSide,
        LeftSide
    }
    
    private static bool _state;
    internal static bool PreviousState;
    internal static bool State {
        get => _state;
        set {
            PreviousState = _state;
            _state = value;
            _ourCam.SetActive(_state);
        } 
    }

    internal static IEnumerator SetupCamera()
    {
        yield return new WaitUntil(() => RootLogic.Instance && RootLogic.Instance.activeCamera);
        _defaultCam = RootLogic.Instance.activeCamera.gameObject;
        _ourCam = new GameObject { gameObject = { name = "ThirdPersonCameraObj" } };
        _ourCam.transform.SetParent(_defaultCam.transform);
        RelocateCam(CameraLocation.Default);
        _ourCam.gameObject.SetActive(false);
        _ourCam.AddComponent<Camera>();
        MelonLogger.Msg("Finished setting up third person camera.");
    }

    internal static void RelocateCam(CameraLocation location, bool resetDist = false)
    {
        _ourCam.transform.rotation = _defaultCam.transform.rotation;
        if(resetDist) ResetDist();
        switch (location)
        {
            case CameraLocation.FrontView:
                _ourCam.transform.localPosition = new Vector3(0, 0.015f, 0.55f + _dist);
                _ourCam.transform.localRotation = new Quaternion(0, 180, 0, 0);
                CurrentLocation = CameraLocation.FrontView;
                break;
            case CameraLocation.RightSide:
                _ourCam.transform.localPosition =  new Vector3(0.3f, 0.015f, -0.55f + _dist);
                _ourCam.transform.localRotation = new Quaternion(0, 0, 0, 0);
                CurrentLocation = CameraLocation.RightSide;
                break;
            case CameraLocation.LeftSide:
                _ourCam.transform.localPosition = new Vector3(-0.3f, 0.015f, -0.55f + _dist);
                _ourCam.transform.localRotation = new Quaternion(0, 0, 0, 0);
                CurrentLocation = CameraLocation.LeftSide;
                break;
            case CameraLocation.Default:
            default:
                _ourCam.transform.localPosition = new Vector3(0, 0.015f, -0.55f + _dist);
                _ourCam.transform.localRotation = new Quaternion(0, 0, 0, 0);
                CurrentLocation = CameraLocation.Default;
                break;
        }
    }
    
    private static void ResetDist() => _dist = 0;
    internal static void IncrementDist() { _dist += 0.25f; RelocateCam(CurrentLocation); }
    internal static void DecrementDist() { _dist -= 0.25f; RelocateCam(CurrentLocation); }
}