using UnityEngine;

public class DebugButton : MonoBehaviour
{
    public void OnButtonClick() =>
        MaxSdk.ShowMediationDebugger();
}