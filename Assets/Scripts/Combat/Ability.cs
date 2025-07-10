using UnityEngine;

namespace MMO.Combat
{
    [CreateAssetMenu(fileName = "Ability", menuName = "MMO/Ability")]
    public class Ability : ScriptableObject
    {
        public string abilityName;
        public float cooldown;
        public GameObject effectPrefab;
    }
}
