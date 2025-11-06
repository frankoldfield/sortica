using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Controller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private XRSimpleInteractable interactable;
    void Start()
    {

        interactable.selectEntered.AddListener(ONControllerSelected);
    }

    public void ONControllerSelected(SelectEnterEventArgs args) 
    {
        MasterScript master = FindFirstObjectByType<MasterScript>();
        master.game_state=GameStates.Start;
    }

}
