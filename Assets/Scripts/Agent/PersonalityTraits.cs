using UnityEngine;



namespace Assets.Scripts.Agent
{

   [System.Serializable]
   public class PersonalityTraits
   {
      [Range(-1f, 1f)] public float aggressiveness;   // Passive (-1) vs Aggressive (1)
      [Range(-1f, 1f)] public float confidence;       // Shy (-1) vs Confident (1)
      [Range(-1f, 1f)] public float emotionalStability; // Mad (-1) vs Calm (1)
      [Range(-1f, 1f)] public float extraversion;     // Introvert (-1) vs Extravert (1)

      // Returns array of all trait values
      public float[] ToArray()
      {
         return new float[]
         {
            aggressiveness,
            confidence,
            emotionalStability,
            extraversion
         };
      }
   }

}