using ABI_RC.Core.Base;
using ABI_RC.Core.Player;
using ABI_RC.Systems.UI;
using Aura2API;
using BeautifyEffect;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.AzureSky;
using UnityEngine.Rendering.PostProcessing;

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
    internal static bool State
    {
        get => _state;
        set
        {
            PreviousState = _state;
            _state = value;
            _ourCam.SetActive(_state);
        }
    }

    private static bool _setupPostProcessing;
    private static readonly FieldInfo ppResources = typeof(PostProcessLayer).GetField("m_Resources", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo ppOldResources = typeof(PostProcessLayer).GetField("m_OldResources", BindingFlags.NonPublic | BindingFlags.Instance);

    internal static IEnumerator SetupCamera()
    {
        yield return new WaitUntil(() => PlayerSetup.Instance && PlayerSetup.Instance.GetActiveCamera());
        _ourCam = new GameObject { gameObject = { name = "ThirdPersonCameraObj" } };
        _ourCam.AddComponent<Camera>();
        //_ourCam.AddComponent<WorldTransitionCamera>();
        ParentCameraObject();
        ThirdPerson.Logger.Msg("Finished setting up third person camera.");
    }

    internal static void ParentCameraObject()
    {
        _defaultCam = PlayerSetup.Instance.GetActiveCamera();
        _ourCam.transform.SetParent(_defaultCam.transform);
        RelocateCam(CameraLocation.Default);
        _ourCam.gameObject.SetActive(false);
        ThirdPerson.Logger.Msg("Parenting ThirdPerson camera object to active camera.");
    }

    internal static void CheckVRSwitch()
    {
        if (_defaultCam != null && _defaultCam != PlayerSetup.Instance.GetActiveCamera())
        {
            ParentCameraObject();
        }
    }

    internal static void CopyFromPlayerCam()
    {
        Camera ourCamComponent = _ourCam.GetComponent<Camera>();
        Camera playerCamComponent = _defaultCam.GetComponent<Camera>();
        if (ourCamComponent == null || playerCamComponent == null) return;
        ThirdPerson.Logger.Msg("Copying active camera settings & components.");

        //steal basic settings
        ourCamComponent.farClipPlane = playerCamComponent.farClipPlane;
        ourCamComponent.nearClipPlane = playerCamComponent.nearClipPlane;
        ourCamComponent.cullingMask = playerCamComponent.cullingMask;
        ourCamComponent.depthTextureMode = playerCamComponent.depthTextureMode;

        //steal post processing if added
        PostProcessLayer ppLayerPlayerCam = playerCamComponent.GetComponent<PostProcessLayer>();
        PostProcessLayer ppLayerThirdPerson = ourCamComponent.AddComponentIfMissing<PostProcessLayer>();
        if (ppLayerPlayerCam != null && ppLayerThirdPerson != null)
        {
            ppLayerThirdPerson.enabled = ppLayerPlayerCam.enabled;
            ppLayerThirdPerson.volumeLayer = ppLayerPlayerCam.volumeLayer;
            //need to copy these via reflection, otherwise post processing will error
            if (!_setupPostProcessing)
            {
                _setupPostProcessing = true;
                PostProcessResources resources = (PostProcessResources)ppResources.GetValue(ppLayerPlayerCam);
                PostProcessResources oldResources = (PostProcessResources)ppOldResources.GetValue(ppLayerPlayerCam);
                ppResources.SetValue(ppLayerThirdPerson, resources);
                ppResources.SetValue(ppLayerThirdPerson, oldResources);
            }
        }

        //what even is this aura camera stuff
        AuraCamera auraPlayerCam = playerCamComponent.GetComponent<AuraCamera>();
        AuraCamera auraThirdPerson = ourCamComponent.AddComponentIfMissing<AuraCamera>();
        if (auraPlayerCam != null && auraThirdPerson != null)
        {
            auraThirdPerson.enabled = auraPlayerCam.enabled;
            auraThirdPerson.frustumSettings = auraPlayerCam.frustumSettings;
        }
        else
        {
            auraThirdPerson.enabled = false;
        }

        //flare layer thing? the sun :_:_:_:_:_:
        FlareLayer flarePlayerCam = playerCamComponent.GetComponent<FlareLayer>();
        FlareLayer flareThirdPerson = ourCamComponent.AddComponentIfMissing<FlareLayer>();
        if (flarePlayerCam != null && flareThirdPerson != null)
        {
            flareThirdPerson.enabled = flarePlayerCam.enabled;
        }
        else
        {
            flareThirdPerson.enabled = false;
        }

        //and now what the fuck is fog scattering
        AzureFogScattering azureFogPlayerCam = playerCamComponent.GetComponent<AzureFogScattering>();
        AzureFogScattering azureFogThirdPerson = ourCamComponent.AddComponentIfMissing<AzureFogScattering>();
        if (azureFogPlayerCam != null && azureFogThirdPerson != null)
        {
            azureFogThirdPerson.fogScatteringMaterial = azureFogPlayerCam.fogScatteringMaterial;
        }
        else
        {
            Object.Destroy(ourCamComponent.GetComponent<AzureFogScattering>());
        }

        //why is there so many thingsssssssss
        Beautify beautifyPlayerCam = playerCamComponent.GetComponent<Beautify>();
        Beautify beautifyThirdPerson = ourCamComponent.AddComponentIfMissing<Beautify>();
        if (beautifyPlayerCam != null && beautifyThirdPerson != null)
        {
            beautifyThirdPerson.quality = beautifyPlayerCam.quality;
            beautifyThirdPerson.profile = beautifyPlayerCam.profile;
        }
        else
        {
            Object.Destroy(ourCamComponent.gameObject.GetComponent<Beautify>());
        }
    }

    internal static void RelocateCam(CameraLocation location, bool resetDist = false)
    {
        _ourCam.transform.rotation = _defaultCam.transform.rotation;
        if (resetDist) ResetDist();
        switch (location)
        {
            case CameraLocation.FrontView:
                _ourCam.transform.localPosition = new Vector3(0, 0.015f, 0.55f + _dist);
                _ourCam.transform.localRotation = new Quaternion(0, 180, 0, 0);
                CurrentLocation = CameraLocation.FrontView;
                break;
            case CameraLocation.RightSide:
                _ourCam.transform.localPosition = new Vector3(0.3f, 0.015f, -0.55f + _dist);
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