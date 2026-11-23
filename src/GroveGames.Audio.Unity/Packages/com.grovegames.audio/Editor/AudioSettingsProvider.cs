using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GroveGames.Audio.Unity.Editor
{
    internal static class AudioSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider("Project/GroveGames/Audio", SettingsScope.Project)
            {
                label = "Audio",
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = AudioSettingsAsset.GetOrCreate();
                    var serializedObject = new SerializedObject(settings);

                    var container = new VisualElement
                    {
                        style =
                        {
                            paddingLeft = 10,
                            paddingRight = 10,
                            paddingTop = 10,
                            paddingBottom = 10
                        }
                    };

                    container.Add(new Label("Audio Settings")
                    {
                        style =
                        {
                            fontSize = 19,
                            unityFontStyleAndWeight = FontStyle.Bold,
                            marginBottom = 10
                        }
                    });

                    var assetField = new ObjectField("Settings Asset")
                    {
                        objectType = typeof(UnityAudioSettings),
                        value = settings,
                        style = { marginBottom = 10 }
                    };
                    assetField.SetEnabled(false);
                    container.Add(assetField);

                    container.Add(new PropertyField(serializedObject.FindProperty("_musicFadeDuration"), "Music Fade Duration"));

                    rootElement.Add(container);
                    rootElement.Bind(serializedObject);
                },
                keywords = new HashSet<string>(new[] { "Audio", "Music", "Sfx", "Fade", "Grove Games" })
            };
        }
    }
}
