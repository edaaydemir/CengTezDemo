// Assets/Scripts/Training/ITrainingEventGenerator.cs
using Assets.Scripts.Agent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Training
{
   /// <summary>
   /// Interface for generating training events for NPC agents
   /// </summary>
   public interface ITrainingEventGenerator
   {
      /// <summary>
      /// Generates and triggers an event for a subset of agents
      /// </summary>
      /// <param name="agents">Available agents to trigger events for</param>
      /// <param name="instigator">The instigator of the event (can be null)</param>
      void GenerateEvent(List<NPCAgent> agents, GameObject instigator);

      /// <summary>
      /// Resets the event generator for a new episode
      /// </summary>
      void Reset();
   }
}
