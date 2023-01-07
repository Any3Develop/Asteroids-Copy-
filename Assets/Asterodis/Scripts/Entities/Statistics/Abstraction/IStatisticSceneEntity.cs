using Services.EntityService;
using UnityEngine;

namespace Asterodis.Entities.Statistics
{
    public interface IStatisticSceneEntity : ISceneEntity, IIndexedEntity
    {
        void SetId(string value);
        void SetTitle(string value);
        void SetText(string value);
        void SetIcon(Sprite value);
        void SetActiveIcon(bool value);
    }
}