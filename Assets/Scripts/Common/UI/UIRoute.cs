using System;

namespace Common.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIRoute : Attribute
    {
        public string Route;
        public UIRoute(string fullRoute)
        {
            Route = fullRoute;
        }
    }
}