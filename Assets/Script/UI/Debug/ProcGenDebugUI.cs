using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ProcGenDebugUI : MonoBehaviour
{
    [SerializeField] Button regenerateButton;
    [SerializeField] TextMeshProUGUI statusDisplay;
    [SerializeField] ProcGenManager targetManager;
    public void OnRegenerate()
    {
        regenerateButton.interactable = false;
        StartCoroutine(PerformRegeneration());
    }

    IEnumerator PerformRegeneration()
    {
        yield return targetManager.AsyncRegenerateWorld(OnStatusReported);

        regenerateButton.interactable = true;
        yield return null;
    }

    void OnStatusReported(EGenerationStage currentStage, string status)
    {
        statusDisplay.text = $"Step {(int)currentStage} of {(int)EGenerationStage.NumStage}: {status}";
    }
}
