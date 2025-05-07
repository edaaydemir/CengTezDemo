// Assets/Scripts/Training/ITrainingDataCollector.cs
using Assets.Scripts.Agent;
using System.Collections.Generic;

namespace Assets.Scripts.Training
{
    /// <summary>
    /// Interface for collecting training data during NPC agent training
    /// </summary>
    public interface ITrainingDataCollector
    {
        /// <summary>
        /// Collects data from the agents at the current state
        /// </summary>
        /// <param name="agents">Agents to collect data from</param>
        /// <param name="episodeNumber">Current episode number</param>
        void CollectData(List<NPCAgent> agents, int episodeNumber);
    }
}
