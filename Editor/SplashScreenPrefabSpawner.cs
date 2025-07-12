#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class SplashScreenPrefabSpawner : MonoBehaviour
    {
        [MenuItem("GameObject/Essentials/Splash Screen", false, priority = 180)]
        private static void SpawnPrefab(MenuCommand menuCommand) =>
            ResourceLoaderEditor.InstantiatePrefab("UnityEssentials_Prefab_SplashScreen", "Splash Screen");
    }
}
#endif