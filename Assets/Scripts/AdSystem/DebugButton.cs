using UnityEngine;

namespace AdSystem
{
    public class DebugButton : MonoBehaviour
    {
        public void OnButtonClick() =>
            MaxSdk.ShowMediationDebugger();
    }
}