#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Asterodis.Entities.Movements;
using Asterodis.Entities.Players;
using Asterodis.Entities.Weapons;
using Newtonsoft.Json;
using Services.SettingsService;
using UnityEditor;
using UnityEngine;

namespace Asterodis.Settings
{
    [CreateAssetMenu(fileName = "ConfigGenerator", menuName = "Game/ConfigGenerator")]
    public class ConfigGenerator : ScriptableObject
    {
        [Header("[==== Main ====]")]
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private AudioSetting audioSetting;
        [SerializeField] private ScreenPortalsSetting portalSetting;
        [SerializeField] private PlayerSetting playerSetting;
        [SerializeField] private PlayerMovementSetting playerMovementSetting;
        
        [Header("[==== Environment ====]")]
        [SerializeField] private PlayerUniverseSetting playerUniverseSetting;
        
        [Header("[==== AI ====]")]
        [SerializeField] private PlayerAiSetting playerAiSetting;
        [SerializeField] private PlayerAiMovementSetting playerAiMovementSetting;
        
        [Header("[==== Weapons ====]")]
        [SerializeField] private AsteroidWeaponSetting asteroidWeaponSetting;
        [SerializeField] private LaserWeaponSetting laserWeaponSetting;
        [SerializeField] private WeaponSetting gunWeaponSetting;
        [SerializeField] private CollisionWeaponSetting collisionWeaponSetting;

        [ContextMenu("Generate")]
        private void Generate()
        {
            var storage = new Dictionary<string, object>
            {
                {nameof(GameSettings), gameSettings},
                {nameof(AudioSetting), audioSetting},
                {nameof(ScreenPortalsSetting), portalSetting},
                {nameof(PlayerSetting), playerSetting},
                {nameof(PlayerMovement), playerMovementSetting},
                {nameof(PlayerUniverse), playerUniverseSetting},
                {nameof(PlayerAiSetting), playerAiSetting},
                {nameof(PlayerAiMovementSetting), playerAiMovementSetting},
                {nameof(AsteroidsWeapon), asteroidWeaponSetting},
                {nameof(LaserWeapon), laserWeaponSetting},
                {nameof(GunWeapon), gunWeaponSetting},
                {nameof(CollisionWeaponSetting), collisionWeaponSetting},
            };

            var wrappers = storage
                .Where(x => x.Value != null && !string.IsNullOrEmpty(x.Key))
                .Select(item => new Wrapper
                {
                    Name = item.Key,
                    Json = JsonConvert.SerializeObject(item.Value),
                    Type = item.Value.GetType()
                }).ToList();

            var path1 = Application.dataPath;
            var path2 = AssetDatabase.GetAssetPath(this);

            path2 = path2.Replace("Assets", "");
            path1 += path2;
            var fullName = Path.GetFileName(path1);
            path1 = path1.Replace(fullName, "") + "GameSettings.json";

            File.WriteAllText(path1, JsonConvert.SerializeObject(wrappers));
        }
    }
}
#endif