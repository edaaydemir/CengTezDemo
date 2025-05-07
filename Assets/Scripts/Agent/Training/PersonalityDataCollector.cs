// Assets/Scripts/Training/PersonalityDataCollector.cs
using Assets.Scripts.Agent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Training
{
    /// <summary>
    /// Collects and logs personality data from NPC agents during training
    /// </summary>
    public class PersonalityDataCollector : ITrainingDataCollector
    {
        private readonly int logFrequency;

        public PersonalityDataCollector(int logFrequency = 100)
        {
            this.logFrequency = logFrequency;
        }

        public void CollectData(List<NPCAgent> agents, int episodeNumber)
        {
            if (agents == null || agents.Count == 0 || episodeNumber % logFrequency != 0)
                return;
                
            string dataLog = $"Episode {episodeNumber} - Personality Analysis:\n";
            
            foreach (NPCAgent agent in agents)
            {
                PersonalityTraits p = agent.personality;
                NPCReaction reaction = agent.CurrentReaction; // Assuming this property exists
                
                dataLog += $"{agent.name}: Aggr={p.aggressiveness:F2}, Conf={p.confidence:F2}, " +
                          $"Emo={p.emotionalStability:F2}, Extr={p.extraversion:F2}, " +
                          $"Last Reaction={reaction}\n";
            }
            
            Debug.Log(dataLog);
        }
    }
}
