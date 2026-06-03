using UnityEngine;

public class ScalesPuzzleController : MonoBehaviour
{
    [SerializeField] private Animator[] shelvesAnimators;

    public void OnScalesSolved()
    {
        StartCoroutine(OpenShelvesDelayed());
    }

    private System.Collections.IEnumerator OpenShelvesDelayed()
    {
        yield return new WaitForSeconds(3f);

        foreach (Animator shelf in shelvesAnimators)
        {
            if (shelf != null)
                shelf.Play("Shelves_Open");
        }
    }
}