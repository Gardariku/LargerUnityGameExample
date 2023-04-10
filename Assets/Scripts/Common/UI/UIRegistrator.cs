using System;
using UnityEngine;

namespace Common.UI
{
    // GameObject with this component should be placed in any scene containing UI windows
    public class UIRegistrator : MonoBehaviour
    {
        private UIWindow[] _windows;
        private void Start()
        {
            _windows = FindObjectsOfType<UIWindow>(true);
            foreach (var window in _windows)
            {
                UIRouter.RegistrateWindow(window);
                window.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            foreach (var window in _windows)
            {
                UIRouter.DeleteWindow(window);
            }
        }
    }
}