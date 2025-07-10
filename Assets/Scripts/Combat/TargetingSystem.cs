using UnityEngine;

namespace MMO.Combat
{
    public class TargetingSystem : MonoBehaviour
    {
        private Transform currentTarget;

        public Transform CurrentTarget => currentTarget;

        public void TargetNext()
        {
            // TODO: implement actual target selection logic
            Debug.Log("Target next not yet implemented.");
        }
    }
}
