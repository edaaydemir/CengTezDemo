using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using TMPro;
using Assets.Scripts.Agent;

public enum TrainingEventType
{
    OnPlayerSpotted,
    OnAttacked,
    OnSteal
}


public class DemoAgentController : Agent
{
    public PersonalityTraits personality;
    private Animator animator;

    [SerializeField] private TextMeshProUGUI personalityText;
    [SerializeField] private TextMeshProUGUI reactionText;
    [SerializeField] private TextMeshProUGUI eventText;

    private NPCReaction currentReaction = NPCReaction.DoNothing;
    private TrainingEventType currentEvent = TrainingEventType.OnPlayerSpotted;

    private float decisionTimer = 0f;
    public float decisionInterval = 10f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GenerateRandomPersonality();
        RandomizeEvent();
        UpdateCanvas();

        RequestDecision(); // Baþlangýçta hemen bir karar iste
    }

    private void Update()
    {
        decisionTimer += Time.deltaTime;

        if (decisionTimer >= decisionInterval)
        {
            RandomizeEvent(); // Her 10 saniyede yeni event gelsin
            RequestDecision();
            decisionTimer = 0f;
        }
    }

    private void GenerateRandomPersonality()
    {
        if (personality == null)
            personality = new PersonalityTraits();

        //personality.aggressiveness = 1f;
        personality.aggressiveness = Random.Range(-1f, 1f);
        personality.confidence = Random.Range(-1f, 1f);
        personality.emotionalStability = Random.Range(-1f, 1f);
        personality.extraversion = Random.Range(-1f, 1f);


    }

    private void RandomizeEvent()
    {
        int eventCount = System.Enum.GetValues(typeof(TrainingEventType)).Length;
        currentEvent = (TrainingEventType)Random.Range(0, eventCount);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (var value in personality.ToArray())
        {
            sensor.AddObservation(value);
        }

        // Event bilgisini de gönderelim
        int eventIndex = (int)currentEvent;
        int eventCount = System.Enum.GetValues(typeof(TrainingEventType)).Length;
        sensor.AddObservation((float)eventIndex / (eventCount - 1)); // normalize edilmiþ
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int actionIndex = actions.DiscreteActions[0];
        currentReaction = (NPCReaction)actionIndex;

        PlayAnimation(currentReaction);
        UpdateCanvas();

        Debug.Log($"NPC reaction: {currentReaction} to {currentEvent}, Personality: Aggr={personality.aggressiveness:F2}, Conf={personality.confidence:F2}, Emo={personality.emotionalStability:F2}, Extr={personality.extraversion:F2}");
    }

    private void PlayAnimation(NPCReaction reaction)
    {
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsGreet", false);
        animator.ResetTrigger("Attack");

        switch (reaction)
        {
            case NPCReaction.Flee:
                animator.SetBool("IsRunning", true);
                break;
            case NPCReaction.Greet:
                animator.SetBool("IsGreet", true);
                break;
            case NPCReaction.Attack:
                animator.SetTrigger("Attack");
                break;
            case NPCReaction.DoNothing:
            default:
                break;
        }
    }

    private void UpdateCanvas()
    {
        if (personalityText != null)
        {
            personalityText.text = $"Aggressiveness: {personality.aggressiveness:F2}\n" +
                                   $"Confidence: {personality.confidence:F2}\n" +
                                   $"Emotional Stabilization: {personality.emotionalStability:F2}\n" +
                                   $"Extraversion: {personality.extraversion:F2}";
        }

        if (reactionText != null)
        {
            string actionDisplay = currentReaction == NPCReaction.DoNothing ? "-" : currentReaction.ToString();
            reactionText.text = $"Action: {actionDisplay}";
        }

        if (eventText != null)
        {
            eventText.text = $"Event: {currentEvent}";
        }
    }
}
