using System;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using BuildInfo = ThirdPerson.BuildInfo;
using static ThirdPerson.CameraLogic;

[assembly: AssemblyCopyright("Created by " + BuildInfo.Author)]
[assembly: MelonInfo(typeof(ThirdPerson.ThirdPerson), BuildInfo.Name, BuildInfo.Version, BuildInfo.Author)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace ThirdPerson;

public static class BuildInfo
{
    public const string Name = "ThirdPerson";
    public const string Author = "Davi";
    public const string Version = "1.0.0";
}

public class ThirdPerson : MelonMod
{
    internal static MelonLogger.Instance Logger;
    
    public override void OnApplicationStart() 
    {
        Logger = LoggerInstance;
        
        MelonCoroutines.Start(SetupCamera());
        
        Patches.Apply(HarmonyInstance);
    }
    
    public override void OnUpdate()
    {
        if(State)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) IncrementDist();
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) DecrementDist();
        }

        if (!Input.GetKey(KeyCode.LeftControl)) return;
        if (Input.GetKeyDown(KeyCode.T)) State = !State;
        if (!State || !Input.GetKeyDown(KeyCode.Y)) return;
        RelocateCam((CameraLocation)(((int)CurrentLocation + 1) % Enum.GetValues(typeof(CameraLocation)).Length), true);
    }
}