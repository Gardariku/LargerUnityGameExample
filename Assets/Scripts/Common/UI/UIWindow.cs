using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.UI
{
    public abstract class UIWindow : MonoBehaviour
    {
        public abstract void Open(Dictionary<string, string> parameters);
        public abstract void Close();
    }
}