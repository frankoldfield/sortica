using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Lever : MonoBehaviour
{

    Animator animator;
    MasterScript master;

    public XRSimpleInteractable interactable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        master = FindFirstObjectByType<MasterScript>();
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnLeverSelected);
    }

    // Update is called once per frame
    void Update()
    {
        
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animStateInfo.IsTag("using") && animStateInfo.normalizedTime >= 0.99f)
        {
            animator.SetBool("Used", false);
        }
    }

    void OnLeverSelected(SelectEnterEventArgs args) 
    {
        
        animator.SetBool("Used", true);

        if (master.game_state.Equals(GameStates.Game_Finished))
        {
            Debug.Log("Exiting the game...");
            master.ExitGame();
        }
        else
        {
            Debug.Log("Restarting the game...");
        }

    }
}
