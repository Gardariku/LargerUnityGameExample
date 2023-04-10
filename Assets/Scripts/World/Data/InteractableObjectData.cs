using UnityEngine;

namespace World.Data
{
    [CreateAssetMenu(fileName = "NewWorldObject", menuName = "World/Objects", order = 0)]
    public class InteractableObjectData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int BaseValue  { get; private set; }
        [field: SerializeField] public InteractableObjectType Type  { get; private set; }
        [field: SerializeField] public InteractableObjectSubtype Subtype { get; private set; }
    }

    public enum InteractableObjectType
    {
        Small,
        Large,
        Guarded
    }
    
    public enum InteractableObjectSubtype
    {
        Sign,
        Artifact,
        Resource,
        Well,
        Tower
    }
}