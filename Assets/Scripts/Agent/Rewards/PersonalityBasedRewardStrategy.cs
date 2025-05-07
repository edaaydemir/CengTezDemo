// Assets/Scripts/Agent/Rewards/PersonalityBasedRewardStrategy.cs
using UnityEngine;

namespace Assets.Scripts.Agent.Rewards
{
   /// <summary>
   /// A reward strategy that evaluates how well an NPC's reaction aligns with their personality traits
   /// </summary>
   public class PersonalityBasedRewardStrategy : IRewardStrategy
   {
      [Tooltip("Base reward multiplier for trait-aligned behavior")]
      private readonly float baseRewardMultiplier;

      [Tooltip("Penalty multiplier for trait-contradicting behavior")]
      private readonly float penaltyMultiplier;

      [Tooltip("Small baseline reward to prevent model collapse")]
      private readonly float baselineReward;

      public PersonalityBasedRewardStrategy(float baseRewardMultiplier = 1.0f, float penaltyMultiplier = 0.8f, float baselineReward = 0.1f)
      {
         this.baseRewardMultiplier = baseRewardMultiplier;
         this.penaltyMultiplier = penaltyMultiplier;
         this.baselineReward = baselineReward;
      }

      public float CalculateReward(PersonalityTraits personality, NPCReaction reaction, TriggerEventType eventType)
      {
         // Start with small baseline reward to ensure exploration
         float reward = baselineReward;

         // Get normalized personality values for easier comparisons
         float aggr = personality.aggressiveness;
         float conf = personality.confidence;
         float emoStab = personality.emotionalStability;
         float extr = personality.extraversion;

         switch (eventType)
         {
            case TriggerEventType.OnPlayerSpotted:
               reward += CalculatePlayerSpottedReward(reaction, aggr, conf, emoStab, extr);
               break;

            case TriggerEventType.OnAttacked:
               reward += CalculateAttackedReward(reaction, aggr, conf, emoStab, extr);
               break;

            case TriggerEventType.OnSteal:
               reward += CalculateTheftReward(reaction, aggr, conf, emoStab, extr);
               break;
         }

         return reward;
      }

      private float CalculatePlayerSpottedReward(NPCReaction reaction, float aggr, float conf, float emoStab, float extr)
      {
         float reward = 0f;

         switch (reaction)
         {
            case NPCReaction.Attack:
               // Aggressive NPCs are more likely to attack when spotting player
               if (aggr > 0.3f)
                  reward += baseRewardMultiplier * aggr;
               // But non-aggressive NPCs get penalized for attacking
               else if (aggr < 0f)
                  reward -= penaltyMultiplier * Mathf.Abs(aggr);
               break;

            case NPCReaction.Greet:
               // Extraverted NPCs more likely to greet
               if (extr > 0.3f)
                  reward += baseRewardMultiplier * extr;
               // Confident NPCs also greet more
               if (conf > 0.3f)
                  reward += baseRewardMultiplier * conf * 0.5f;
               // But introverts get penalized for greeting
               if (extr < -0.3f)
                  reward -= penaltyMultiplier * Mathf.Abs(extr);
               break;

            case NPCReaction.Flee:
               // Shy and passive NPCs might flee
               if (conf < -0.3f)
                  reward += baseRewardMultiplier * Mathf.Abs(conf);
               if (aggr < -0.3f)
                  reward += baseRewardMultiplier * Mathf.Abs(aggr) * 0.7f;
               // But confident NPCs get penalized for fleeing
               if (conf > 0.5f)
                  reward -= penaltyMultiplier * conf;
               break;

            case NPCReaction.DoNothing:
               // Calm NPCs might do nothing
               if (emoStab > 0.3f)
                  reward += baseRewardMultiplier * emoStab * 0.8f;
               // Introverted NPCs might do nothing
               if (extr < -0.3f)
                  reward += baseRewardMultiplier * Mathf.Abs(extr) * 0.6f;
               break;
         }

         return reward;
      }

      private float CalculateAttackedReward(NPCReaction reaction, float aggr, float conf, float emoStab, float extr)
      {
         float reward = 0f;

         switch (reaction)
         {
            case NPCReaction.Attack:
               // Aggressive and confident NPCs more likely to counterattack
               if (aggr > 0f)
                  reward += baseRewardMultiplier * aggr;
               if (conf > 0f)
                  reward += baseRewardMultiplier * conf * 0.5f;
               // Low emotional stability NPCs might attack rashly
               if (emoStab < -0.5f)
                  reward += baseRewardMultiplier * Mathf.Abs(emoStab) * 0.5f;
               // But very passive NPCs get penalized for attacking
               if (aggr < -0.7f)
                  reward -= penaltyMultiplier * Mathf.Abs(aggr);
               break;

            case NPCReaction.Flee:
               // Non-aggressive or non-confident NPCs more likely to flee
               if (aggr < 0f)
                  reward += baseRewardMultiplier * Mathf.Abs(aggr);
               if (conf < 0f)
                  reward += baseRewardMultiplier * Mathf.Abs(conf) * 0.7f;
               // But very aggressive and confident NPCs get penalized for fleeing
               if (aggr > 0.7f && conf > 0.7f)
                  reward -= penaltyMultiplier * ((aggr + conf) / 2);
               break;

            case NPCReaction.DoNothing:
               // Emotionally stable NPCs might not overreact
               if (emoStab > 0.5f)
                  reward += baseRewardMultiplier * emoStab;
               // But unstable NPCs get penalized for doing nothing
               if (emoStab < -0.5f)
                  reward -= penaltyMultiplier * Mathf.Abs(emoStab) * 0.7f;
               break;

            case NPCReaction.Greet:
               // Generally inappropriate to greet an attacker
               reward -= penaltyMultiplier * 0.8f;
               // Unless extremely extraverted and stable
               if (extr > 0.8f && emoStab > 0.8f)
                  reward += baseRewardMultiplier * 0.3f;
               break;
         }

         return reward;
      }

      private float CalculateTheftReward(NPCReaction reaction, float aggr, float conf, float emoStab, float extr)
      {
         float reward = 0f;

         switch (reaction)
         {
            case NPCReaction.Attack:
               // Aggressive NPCs more likely to attack thieves
               if (aggr > 0.3f)
                  reward += baseRewardMultiplier * aggr;
               // Low emotional stability may lead to attack
               if (emoStab < -0.3f)
                  reward += baseRewardMultiplier * Mathf.Abs(emoStab) * 0.5f;
               // But very passive NPCs get penalized for attacking
               if (aggr < -0.7f)
                  reward -= penaltyMultiplier * Mathf.Abs(aggr) * 0.7f;
               break;

            case NPCReaction.DoNothing:
               // Non-confident NPCs might do nothing
               if (conf < -0.2f)
                  reward += baseRewardMultiplier * Mathf.Abs(conf) * 0.7f;
               // Calm NPCs might not overreact
               if (emoStab > 0.5f)
                  reward += baseRewardMultiplier * emoStab * 0.5f;
               break;

            case NPCReaction.Flee:
               // Non-aggressive NPCs might flee
               if (aggr < -0.3f)
                  reward += baseRewardMultiplier * Mathf.Abs(aggr);
               // Non-confident NPCs might flee
               if (conf < -0.3f)
                  reward += baseRewardMultiplier * Mathf.Abs(conf) * 0.7f;
               break;

            case NPCReaction.Greet:
               // Generally inappropriate to greet a thief
               reward -= penaltyMultiplier;
               break;
         }

         return reward;
      }
   }
}