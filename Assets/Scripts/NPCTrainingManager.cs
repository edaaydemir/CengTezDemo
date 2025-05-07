// Assets/Scripts/NPCTrainingManager.cs
using Assets.Scripts.Training;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
   public class NPCTrainingManager : MonoBehaviour
   {
      [Header("Training Settings")]
      [SerializeField] private TrainingEnvironmentManager environmentManager;
      [SerializeField] private bool isTraining = true;
      [SerializeField] private int maxEpisodes = 10000;
      [SerializeField] private float episodeDuration = 5f;

      [Header("Event Generation")]
      [SerializeField] private TriggerEventType[] trainingEvents;
      [SerializeField] private int eventsPerEpisode = 2;
      [SerializeField] private float eventDelay = 1.0f;

      [Header("Analysis")]
      [SerializeField] private bool collectPersonalityData = true;
      [SerializeField] private int logFrequency = 100;

      // Private fields for DI components
      private ITrainingEventGenerator eventGenerator;
      private ITrainingDataCollector dataCollector;

      private int episodeCount = 0;
      private float episodeTimer = 0f;
      private bool trainingActive = false;
      private float nextEventTime = 0f;

      private void Awake()
      {
         // Initialize default components if they haven't been injected
         InitializeComponents();
      }

      void Start()
      {
         // Speed up training
         Time.timeScale = 10.0f;
         Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;

         if (environmentManager == null)
            environmentManager = GetComponent<TrainingEnvironmentManager>();

         if (environmentManager == null)
         {
            Debug.LogError("No environment manager found. Cannot start training.");
            return;
         }

         // If no training events are specified, use all available events
         if (trainingEvents == null || trainingEvents.Length == 0)
         {
            trainingEvents = System.Enum.GetValues(typeof(TriggerEventType))
                .Cast<TriggerEventType>()
                .ToArray();
         }

         if (isTraining)
            StartTraining();
      }

      private void InitializeComponents()
      {
         if (eventGenerator == null)
            eventGenerator = new RandomEventGenerator(trainingEvents, eventsPerEpisode);

         if (dataCollector == null && collectPersonalityData)
            dataCollector = new PersonalityDataCollector(logFrequency);
      }

      // These methods allow for dependency injection
      public void SetEventGenerator(ITrainingEventGenerator generator)
      {
         eventGenerator = generator;
      }

      public void SetDataCollector(ITrainingDataCollector collector)
      {
         dataCollector = collector;
      }

      void Update()
      {
         if (!trainingActive) return;

         episodeTimer += Time.deltaTime;

         // Trigger training events during episode
         if (Time.time >= nextEventTime && eventGenerator != null)
         {
            eventGenerator.GenerateEvent(environmentManager.Agents.ToList(), gameObject);
            nextEventTime = Time.time + eventDelay;
         }

         if (episodeTimer >= episodeDuration)
         {
            EndCurrentEpisode();

            if (episodeCount < maxEpisodes)
               StartNewEpisode();
            else
               EndTraining();
         }
      }

      public void StartTraining()
      {
         episodeCount = 0;
         trainingActive = true;
         StartNewEpisode();
         Debug.Log("Starting NPC personality training");
      }

      private void StartNewEpisode()
      {
         episodeCount++;
         episodeTimer = 0f;
         nextEventTime = eventDelay; // First event after delay

         if (eventGenerator != null)
            eventGenerator.Reset();

         environmentManager.StartNewEpisode();

         // Collect data if needed
         if (dataCollector != null)
            dataCollector.CollectData(environmentManager.Agents.ToList(), episodeCount);

         if (episodeCount % logFrequency == 0)
            Debug.Log($"Starting episode {episodeCount}/{maxEpisodes}");
      }

      private void EndCurrentEpisode()
      {
         environmentManager.EndEpisode();
      }

      private void EndTraining()
      {
         trainingActive = false;
         Debug.Log("NPC personality training completed!");

         // Final data collection
         if (dataCollector != null)
            dataCollector.CollectData(environmentManager.Agents.ToList(), episodeCount);

         // Reset timescale
         Time.timeScale = 1.0f;
         Time.fixedDeltaTime = 0.02f;
      }
   }
}