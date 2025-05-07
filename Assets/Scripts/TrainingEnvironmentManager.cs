// Assets/Scripts/TrainingEnvironmentManager.cs
using Assets.Scripts.Agent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
   public class TrainingEnvironmentManager : MonoBehaviour
   {
      [Header("NPC Prefabs")]
      public GameObject npcAgentPrefab;

      [Header("Training Environment")]
      public int numberOfAgents = 10;
      public float areaSize = 20f;
      public Transform environmentParent;

      [Header("Training Settings")]
      public bool autoStartTraining = true;
      public float episodeDuration = 10f;

      // Make agents public read-only through a property
      private List<NPCAgent> _agents = new List<NPCAgent>();
      public IReadOnlyList<NPCAgent> Agents => _agents.AsReadOnly();

      private float episodeTimer = 0f;
      private bool episodeActive = false;

      void Start()
      {
         if (environmentParent == null)
            environmentParent = transform;

         SetupEnvironment();

         if (autoStartTraining)
            StartNewEpisode();
      }

      void Update()
      {
         if (!episodeActive) return;

         episodeTimer += Time.deltaTime;
         if (episodeTimer >= episodeDuration)
         {
            EndEpisode();
            StartNewEpisode();
         }
      }

      private void SetupEnvironment()
      {
         // Clean up existing environment
         foreach (Transform child in environmentParent)
         {
            if (Application.isPlaying)
               Destroy(child.gameObject);
            else
               DestroyImmediate(child.gameObject);
         }

         _agents.Clear();

         // Create NPCs
         for (int i = 0; i < numberOfAgents; i++)
         {
            Vector3 position = new Vector3(
                Random.Range(-areaSize / 2, areaSize / 2),
                Random.Range(-areaSize / 2, areaSize / 2),
                0
            );

            GameObject npcObject = Instantiate(npcAgentPrefab, position, Quaternion.identity, environmentParent);
            npcObject.name = $"NPC_{i + 1}";

            NPCAgent agent = npcObject.GetComponent<NPCAgent>();
            if (agent != null)
            {
               _agents.Add(agent);
            }
            else
            {
               Debug.LogError("NPCAgent component not found on instantiated prefab!");
            }
         }
      }

      public void StartNewEpisode()
      {
         episodeTimer = 0f;
         episodeActive = true;

         foreach (NPCAgent agent in _agents)
         {
            agent.OnEpisodeBegin();
         }
      }

      public void EndEpisode()
      {
         episodeActive = false;

         foreach (NPCAgent agent in _agents)
         {
            agent.EndEpisode();
         }
      }
   }
}
