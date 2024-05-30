using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerVisual : MonoBehaviourPun
{
    public Renderer PlayerRenderer;
    public Material BlueTeamMaterial;
    public Material OrangeTeamMaterial;

    private const string TeamPropertyKey = "Team";

    void Start()
    {
        UpdateTeamVisual();
    }

    void UpdateTeamVisual()
    {
        if (photonView.Owner.CustomProperties.ContainsKey(TeamPropertyKey))
        {
            string team = photonView.Owner.CustomProperties[TeamPropertyKey].ToString();
            if (team == "Blue")
            {
                PlayerRenderer.material = BlueTeamMaterial;
            }
            else if (team == "Orange")
            {
                PlayerRenderer.material = OrangeTeamMaterial;
            }
        }
    }
}