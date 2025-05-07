// Assets/Scripts/Agent/Rewards/IRewardStrategy.cs
using UnityEngine;

namespace Assets.Scripts.Agent.Rewards
{
    /// <summary>
    /// Interface for defining reward calculation strategies
    /// </summary>
    public interface IRewardStrategy
    {
        /// <summary>
        /// Calculates a reward based on an NPC's reaction to an event
        /// </summary>
        /// <param name="personality">The personality traits of the NPC</param>
        /// <param name="reaction">The chosen reaction</param>
        /// <param name="eventType">The type of event that triggered the reaction</param>
        /// <returns>A float value representing the reward</returns>
        float CalculateReward(PersonalityTraits personality, NPCReaction reaction, TriggerEventType eventType);
    }
}
