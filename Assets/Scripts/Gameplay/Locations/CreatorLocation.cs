using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gameplay.Buildings;
using Gameplay.Characters;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Locations
{
    //used only in edit mode
    public class CreatorLocation : MonoBehaviour
    {
        private const string DefaultPath = "Scripts/Gameplay/Locations/Settings";
        private const string DefaultExt = "json";
        private const string DefaultName = "Location";
        private const float MovementBoundsRadius = 10;

        [SerializeField] private List<LocationSettings> _settings = new();
        
        public void LoadSettings()
        {
            var files = Directory.GetFiles(Path.Combine(UnityEngine.Application.dataPath, DefaultPath), $"*.{DefaultExt}");
            _settings.Clear();
            foreach (var file in files)
            {
                var location = JsonUtility.FromJson<LocationSettings>(File.ReadAllText(file));
                _settings.Add(location);
            }
        }
        
        public static string[] GetLocationNames() => Directory.GetFiles(Path.Combine(UnityEngine.Application.dataPath, DefaultPath), $"*.{DefaultExt}");
        

        public void SaveByScene()
        {
            var buildings = FindObjectsOfType<BuildingView>();
            var character = FindObjectOfType<CharacterView>();
            var camera = FindObjectOfType<Camera>();

            var settings = new LocationSettings()
            {
                Buildings = buildings.Select(b => b.GetLocationSettingsData()).ToArray(),
                Character = new LocationSettings.Point()
                {
                    Position = character.transform.position,
                    Rotation = character.transform.rotation.eulerAngles
                },
                Camera = new LocationSettings.Point()
                {
                    Position = camera.transform.position,
                    Rotation = camera.transform.rotation.eulerAngles
                },
                MovementBboundsRadius = MovementBoundsRadius
            };
            var path = EditorUtility.SaveFilePanel(DefaultName, Path.Combine(UnityEngine.Application.dataPath, DefaultPath), DefaultName, DefaultExt);
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, JsonUtility.ToJson(settings));
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [ContextMenu("Custom Tools/CreatorLocation Activate")]
        public static void CreatorLocationActivate()
        {
            //no implementation
        }
    }
    
    #if UNITY_EDITOR
    
    [CustomEditor(typeof(CreatorLocation))]
    public class CreatorLocationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CreatorLocation myComponent = (CreatorLocation)target;
            
            if (GUILayout.Button("LoadSettings"))
                myComponent.LoadSettings();
            
            if (GUILayout.Button("SaveByScene"))
                myComponent.SaveByScene();
        }
    }
    
    #endif
}