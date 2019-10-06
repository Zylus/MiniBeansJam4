using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission
{
    private List<string> CARGO_NAMES = new List<string>()
    {
        "Marsian Powder", "Highly Scientific Waste",
        "Hacked Cyberware", "Relaxing Herbs", "Placebos (Extra Dosage)"
    };

    private const int MIN_REWARD = 100;
    private const int MAX_REWARD = 5000;
    private const float MIN_DISTANCE = 10f;
    private const float MAX_DISTANCE = 100f;

    public Station StartStation { get; set; }
    public Station EndStation { get; set; }
    public int Reward { get; set; }
    public string CargoName { get; set; }
    
    public void Init(Station startStation)
    {
        StartStation = startStation;
        List<Station> possibleTargets = new List<Station>(GameController.Instance.Stations);
        possibleTargets.Remove(startStation);
        EndStation = possibleTargets[Random.Range(0, possibleTargets.Count)];
        float distance = Vector2.Distance(StartStation.transform.position, EndStation.transform.position);
        float percentage = Mathf.InverseLerp(MIN_DISTANCE, MAX_DISTANCE, distance);
        percentage = percentage * Random.Range(0.8f, 1.2f);
        Reward = (int)Mathf.Lerp(MIN_REWARD, MAX_REWARD, percentage);
        CargoName = CARGO_NAMES[Random.Range(0, CARGO_NAMES.Count)];
    }
}
