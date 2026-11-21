using UnityEngine;

// Empty MonoBehaviour used only to run coroutines on a helper GameObject.
// Unity 2019 refuses AddComponent<ResurfCoroutineRunner>() (base class), which broke startup:
// the null runner stopped SSEventsManagerController, character setup and asset bundles.
public class ResurfCoroutineRunner : MonoBehaviour
{
}
