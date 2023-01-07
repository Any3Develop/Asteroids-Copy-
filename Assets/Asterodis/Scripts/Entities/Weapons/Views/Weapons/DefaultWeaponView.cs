using System;
using UnityEngine;

namespace Asterodis.Entities.Weapons
{
    public class DefaultWeaponView : MonoBehaviour, IWeaponSceneEntity
    {
        [SerializeField] private Transform selfContainer;
        [SerializeField] private Transform aimContainer;
        public string Id { get; private set; }
        
        public Transform Container => selfContainer;
        
        public Transform Aim => aimContainer;

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