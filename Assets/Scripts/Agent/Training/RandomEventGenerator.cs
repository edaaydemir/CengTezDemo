// Assets/Scripts/Training/RandomEventGenerator.cs
using Assets.Scripts.Agent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Training
{
   /// <summary>
   /// Generates random events for training NPC agents
   /// </summary>
   public class RandomEventGenerator : ITrainingEventGenerator
   {
      private readonly TriggerEventType[] eventTypes;
      private readonly int eventsPerEpisode;
      private int eventsTriggered;

      public RandomEventGenerator(TriggerEventType[] eventTypes, int eventsPerEpisode = 2)
      {
         this.eventTypes = eventTypes;
         this.eventsPerEpisode = 3;
         this.eventsTriggered = 0;
      }

      public void GenerateEvent(List<NPCAgent> agents, GameObject instigator)
      {
         if (agents == null || agents.Count == 0 || eventsTriggered >= eventsPerEpisode)
         {
            Debug.LogWarning($"No agents available or event limit reached  (${eventsTriggered}/{eventsPerEpisode}).");
            return;
         }

         TriggerEventType eventType = eventTypes[eventsTriggered % eventTypes.Length];
         Debug.LogWarning($"{eventType} Triggered for all agents.");

         for (int i = 0; i < agents.Count; i++)
         {
            agents[i].HandleEvent(eventType, instigator);
         }
         eventsTriggered++;
      }

      public void Reset()
      {
         eventsTriggered = 0;
         Debug.Log("Event generator reset for new episode.");
      }
   }
}
