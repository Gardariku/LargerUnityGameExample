using UnityEngine;

namespace Battle.Controller.Field
{
    public interface IFieldObject
    {
        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; }
        public int Speed { get; }
    }
}