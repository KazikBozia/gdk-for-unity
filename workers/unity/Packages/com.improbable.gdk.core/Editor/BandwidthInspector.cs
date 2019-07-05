using System.Linq;
using Improbable.Gdk.Core;
using UnityEditor;
using UnityEngine;
using Unity.Entities;

public class BandwidthInspector : EditorWindow
{
    private int worldIndex = 0;

    [MenuItem("SpatialOS/Analysis/Bandwidth Inspector", false)]
    private static void BandwidthInspectorMenu()
    {
        var inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
        var inspector = GetWindow<BandwidthInspector>(inspectorWindowType);
        inspector.titleContent.text = "Bandwidth Inspector";
        inspector.titleContent.tooltip = "A tab for inspector SpatialOS bandwidth usage.";
        inspector.Show();
    }

    public void OnInspectorUpdate()
    {
        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("This only works in play mode.", MessageType.Info);
            return;
        }

        var worlds = World.AllWorlds.Where(w => w.GetExistingSystem<ComponentUpdateSystem>() != null).ToList().AsReadOnly();
        if (worlds.Count == 0)
        {
            EditorGUILayout.HelpBox("No SpatialOS worlds found.", MessageType.Warning);
            return;
        }

        if (worldIndex >= worlds.Count)
        {
            worldIndex = 0;
        }

        worldIndex = EditorGUILayout.Popup(worldIndex, worlds.Select(w => w.Name).ToArray());
        var selectedWorld = worlds[worldIndex];
        var componentUpdateSystem = selectedWorld.GetExistingSystem<ComponentUpdateSystem>();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Component", "Total Count");
        EditorGUILayout.Space();
        foreach (var pair in componentUpdateSystem.ComponentSendCount)
        {
            var componentName = pair.Key.DeclaringType.Name;
            var count = pair.Value;

            EditorGUILayout.LabelField(componentName, $"{count}");
        }
    }
}
