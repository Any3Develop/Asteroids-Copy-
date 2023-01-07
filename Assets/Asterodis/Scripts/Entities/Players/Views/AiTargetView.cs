using Services.LoggerService;
using UnityEngine;

namespace Asterodis.Entities.Players
{
    public class AiTargetView : MonoBehaviour, IAiTargetSceneEntity
    {
        public string Id { get; private set; }
        public Transform Container => transform;
        public bool IsValidTarget => true;

        public void SetOwnerId(string id)
        {
            Id = id;
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}