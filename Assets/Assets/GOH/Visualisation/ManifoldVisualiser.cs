using UnityEngine;

namespace GOH
{
    public class ManifoldVisualiser : MonoBehaviour
    {
        public static ManifoldVisualiser instance;

        private void Start()
        {
            if (instance != null)
            {
                UnityEngine.Debug.LogError("ManifoldVisualiser already instantiated.");
                return;
            }
            instance = this;
        }
    }
}