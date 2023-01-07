using UnityEngine;

namespace Services.UIService
{
    [CreateAssetMenu(fileName = "AnimationHelperPreset", menuName = "Config/AnimationHelperPreset", order = 0)]
    public class AnimationHelperPreset : ScriptableObject
    {
        public AnimationSettings Settings => settings;
        [SerializeField] private AnimationSettings settings;
    }
}