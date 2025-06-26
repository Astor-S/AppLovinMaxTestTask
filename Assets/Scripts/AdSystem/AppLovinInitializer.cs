using UnityEngine;

namespace AdSystem
{
    public class AppLovinInitializer : MonoBehaviour
    {
        private void Start()
        {
            MaxSdkUnityEditor.InitializeSdk();
        }
    }
}