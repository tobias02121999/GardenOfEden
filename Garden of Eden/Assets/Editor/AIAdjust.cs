using UnityEngine;
using UnityEditor;

public class AIAjuster : EditorWindow
{
    GameObject human;
    string name;
    float speed, wanderDuration, turnSpeed;
    bool groupEnabled, proneToDepression, Gluttenous, unfaithful;

    [MenuItem("Window/AI/AI Adjustment Tool")]
    public static void ShowWindow() { EditorWindow.GetWindow(typeof(AIAjuster)); }

    void OnGUI()
    {
        GUILayout.Label("Adjust AI", EditorStyles.boldLabel);
        human = EditorGUILayout.ObjectField("Target AI (GameObject):", human, typeof(GameObject), false) as GameObject;

        GUILayout.Space(5f);

        name = EditorGUILayout.TextField("Name:", name);

        GUILayout.Space(10f);

        speed = EditorGUILayout.Slider("Speed", speed, 0, 20);
        wanderDuration = EditorGUILayout.Slider("Wander Duration", wanderDuration, 50, 500);
        turnSpeed = EditorGUILayout.Slider("Turn speed", turnSpeed, 0, 20);

        GUILayout.Space(10f);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Imperfection?", groupEnabled);
            proneToDepression = EditorGUILayout.Toggle("Prone to depression", proneToDepression);
            Gluttenous = EditorGUILayout.Toggle("Gluttenous", Gluttenous);
            unfaithful = EditorGUILayout.Toggle("Unfaithful", unfaithful);
        EditorGUILayout.EndToggleGroup();

        GUILayout.Space(10f);

        if (GUILayout.Button("Instantiate AI"))
        {
            if (speed == 0 || turnSpeed == 0)
                Debug.LogWarning("Some AI values are set to zero!");
            if (name == null)
                Debug.LogWarning("This unit's name has not been set!");

            GameObject ai = Instantiate(human, Vector3.zero, Quaternion.identity);
            HumanAI humanAI = ai.GetComponent<HumanAI>();

            humanAI.name = name;
            humanAI.speed = speed;
            humanAI.wanderDuration = wanderDuration;
            humanAI.turnSpeed = turnSpeed;

            if (proneToDepression)
                   // (Increase chance to get depression, yet to be implemented.)
            if (Gluttenous)
                  // (Make the human count twice as much for the food calculation, yet to be implemented.)
            if (unfaithful)                
                  humanAI.IncreaseFaithOverTime(true);
        }
    }
}