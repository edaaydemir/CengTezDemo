// Assets/Scripts/Agent/NPCAgent.cs
using Assets.Scripts.Agent.Rewards;
using System;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Assets.Scripts.Agent
{
   public class NPCAgent : Unity.MLAgents.Agent
   {
      // ***** PERSONALITY *****
      [Header("Personality")]
      public PersonalityTraits personality;

      [Header("Decision Making")]
      [SerializeField] private NPCReaction currentReaction = NPCReaction.DoNothing;

      // Property to expose current reaction publicly but only allow internal setting
      public NPCReaction CurrentReaction => currentReaction;

      private TriggerEventType currentEventType;
      private GameObject eventInstigator;
      private IRewardStrategy rewardStrategy;

      private void Awake()
      {
         if (personality == null)
         {
            personality = new PersonalityTraits();
         }

         personality.extraversion = UnityEngine.Random.Range(-1f, 1.0f);
         personality.aggressiveness = UnityEngine.Random.Range(-1f, 1.0f);
         personality.confidence = UnityEngine.Random.Range(-1f, 1.0f);
         personality.emotionalStability = UnityEngine.Random.Range(-1f, 1.0f);

         // Default reward strategy - can be replaced via dependency injection
         rewardStrategy = new PersonalityBasedRewardStrategy();
      }

      // Allow setting a custom reward strategy (dependency injection)
      public void SetRewardStrategy(IRewardStrategy strategy)
      {
         rewardStrategy = strategy ?? new PersonalityBasedRewardStrategy();
      }

      public override void OnEpisodeBegin()
      {
         // Reset reaction state
         currentReaction = NPCReaction.DoNothing;
         currentEventType = TriggerEventType.OnPlayerSpotted; // Default
         eventInstigator = null;
      }

      // Define the observation space
      public override void CollectObservations(VectorSensor sensor)
      {
         // Observe personality traits (4 values)
         sensor.AddObservation(personality.aggressiveness);
         sensor.AddObservation(personality.confidence);
         sensor.AddObservation(personality.emotionalStability);
         sensor.AddObservation(personality.extraversion);

         // Observe current event (one-hot encoded)
         int eventTypeCount = Enum.GetValues(typeof(TriggerEventType)).Length;
         for (int i = 0; i < eventTypeCount; i++)
         {
            sensor.AddObservation(i == (int)currentEventType ? 1.0f : 0.0f);
         }

         // Observe previous reaction
         int reactionCount = Enum.GetValues(typeof(NPCReaction)).Length;
         for (int i = 0; i < reactionCount; i++)
         {
            sensor.AddObservation(i == (int)currentReaction ? 1.0f : 0.0f);
         }
      }

      public override void OnActionReceived(ActionBuffers actionBuffers)
      {
         // Get discrete action (represents the NPC reaction type)
         int actionIndex = actionBuffers.DiscreteActions[0];
         currentReaction = (NPCReaction)(actionIndex % System.Enum.GetValues(typeof(NPCReaction)).Length);

         // Apply rewards based on reaction and personality alignment
         if (rewardStrategy != null)
         {
            float reward = rewardStrategy.CalculateReward(personality, currentReaction, currentEventType);
            AddReward(reward);

            // Debug information
            Debug.Log($"NPC reaction: {currentReaction.ToString()} to {currentEventType.ToString()}, Reward: {reward}, " +
                     $"Personality: Aggr={personality.aggressiveness:F2}, Conf={personality.confidence:F2}, " +
                     $"Emo={personality.emotionalStability:F2}, Extr={personality.extraversion:F2}");
         }
      }

      // For testing behaviors via keyboard
      private void Update()
      {
         if (Input.GetKeyDown(KeyCode.A))
         {
            HandleEvent(TriggerEventType.OnAttacked, null);
         }
         if (Input.GetKeyDown(KeyCode.S))
         {
            HandleEvent(TriggerEventType.OnSteal, null);
         }
         if (Input.GetKeyDown(KeyCode.D))
         {
            HandleEvent(TriggerEventType.OnPlayerSpotted, null);
         }
      }

      // ***** PUBLIC API *****

      // Handle an event that the NPC should react to
      public void HandleEvent(TriggerEventType eventType, GameObject instigator)
      {
         currentEventType = eventType;
         eventInstigator = instigator;
         Debug.Log($"Event started: {eventType} - {instigator?.name}");
         RequestDecision();
      }

      // Shorthand methods for triggering specific events
      public void PlayerSpotted(GameObject player) => HandleEvent(TriggerEventType.OnPlayerSpotted, player);
      public void TakeDamage(GameObject attacker) => HandleEvent(TriggerEventType.OnAttacked, attacker);
      public void ItemStolen(GameObject thief) => HandleEvent(TriggerEventType.OnSteal, thief);
   }
}