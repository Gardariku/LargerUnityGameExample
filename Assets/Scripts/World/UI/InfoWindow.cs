using System.Collections.Generic;
using Common.UI;
using TMPro;
using UnityEngine;

namespace World.UI
{
    [UIRoute("info")]
    public class InfoWindow : UIWindow
    {
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _message;
        
        public override void Open(Dictionary<string, string> parameters)
        {
            _header.text = parameters.GetValueOrDefault("header", "");
            if (parameters.TryGetValue("message", out var text))
                _message.text = text;
            else
                Debug.LogError("Parameters for Info Window don't contain message text.");
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}