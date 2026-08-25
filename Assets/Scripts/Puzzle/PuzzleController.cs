using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public enum LogicType
    {
        AND,
        OR
    }

    [SerializeField] private List<PuzzleInput> inputs = new();
    [SerializeField] private List<PuzzleOutput> outputs = new();

    [SerializeField] private LogicType logicType = LogicType.AND;

    [SerializeField] private bool isSatisfied = false;

    public void InputChanged()
    {
        EvaluateConditions();
    }

    private void EvaluateConditions()
    {
        bool result;

        if (logicType == LogicType.AND)
        {
            result = true;

            foreach (PuzzleInput input in inputs)
            {
                if (!input.GetCondition())
                {
                    result = false;
                    break;
                }
            }
        }
        else
        {
            result = false;

            foreach (PuzzleInput input in inputs)
            {
                if (input.GetCondition())
                {
                    result = true;
                    break;
                }
            }
        }

        if (result != isSatisfied)
        {
            isSatisfied = result;
            NotifyOutputs();
        }
    }

    private void NotifyOutputs()
    {
        foreach (PuzzleOutput output in outputs)
        {
            output.ReceiveNotification(isSatisfied);
        }
    }
}