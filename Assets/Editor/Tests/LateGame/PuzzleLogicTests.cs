using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class LateGameFakeInput : PuzzleInput
    {
        public override void ResetPuzzleObject() => SetActive(false);
        public void Link(PuzzleController controller) => puzzleController = controller;
    }
public class LateGameFakeOutput : PuzzleOutput
    {
        public int Activations, Deactivations;
        public bool IsActive => currentState;
        protected override void Activate() => Activations++;
        protected override void Deactivate() => Deactivations++;
        public void Configure(bool invert, bool initial)
        {
            invertOutput = invert;
            startingState = initial;
        }
    }
public class PuzzleLogicTests
{
    private static void SetField(object target, string field, object value)
    {
        typeof(PuzzleController).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    }
    private static void Configure(PuzzleController controller, PuzzleController.LogicType logic, List<PuzzleInput> inputs, List<PuzzleOutput> outputs)
    {
        SetField(controller, "logicType", logic);
        SetField(controller, "inputs", inputs);
        SetField(controller, "outputs", outputs);
    }

    [Test] public void Input_StateChangesOnlyWhenValueChanges()
    {
        var go = new GameObject("input");
        try
        {
            var input = go.AddComponent<LateGameFakeInput>();
            Assert.IsFalse(input.GetCondition());
            input.SetActive(true);
            Assert.IsTrue(input.GetCondition());
            input.SetActive(true);
            Assert.IsTrue(input.GetCondition());
            input.ResetPuzzleObject();
            Assert.IsFalse(input.GetCondition());
        }
        finally { Object.DestroyImmediate(go); }
    }

    [TestCase(PuzzleController.LogicType.AND, false, false, false)]
    [TestCase(PuzzleController.LogicType.AND, true, false, false)]
    [TestCase(PuzzleController.LogicType.AND, true, true, true)]
    [TestCase(PuzzleController.LogicType.OR, false, false, false)]
    [TestCase(PuzzleController.LogicType.OR, true, false, true)]
    [TestCase(PuzzleController.LogicType.OR, true, true, true)]
    public void Controller_EvaluatesTwoInputs(PuzzleController.LogicType logic, bool a, bool b, bool expected)
    {
        var go = new GameObject("puzzle");
        try
        {
            var controller = go.AddComponent<PuzzleController>();
            var first = go.AddComponent<LateGameFakeInput>();
            var second = go.AddComponent<LateGameFakeInput>();
            var output = go.AddComponent<LateGameFakeOutput>();
            Configure(controller, logic, new List<PuzzleInput>{first, second}, new List<PuzzleOutput>{output});
            first.Link(controller);
            second.Link(controller);
            first.SetActive(a);
            second.SetActive(b);
            controller.InputChanged();
            Assert.AreEqual(expected, output.IsActive);
        }
        finally { Object.DestroyImmediate(go); }
    }

    [Test] public void Output_InvertsNotificationsAndAvoidsDuplicateActivation()
    {
        var go = new GameObject("output");
        try
        {
            var output = go.AddComponent<LateGameFakeOutput>();
            output.Configure(true, false);
            output.ReceiveNotification(false);
            output.ReceiveNotification(false);
            Assert.IsTrue(output.IsActive);
            Assert.AreEqual(1, output.Activations);
            output.ReceiveNotification(true);
            Assert.IsFalse(output.IsActive);
            Assert.AreEqual(1, output.Deactivations);
        }
        finally { Object.DestroyImmediate(go); }
    }

    [Test] public void Output_ResetRestoresConfiguredStartingState()
    {
        var go = new GameObject("output");
        try
        {
            var output = go.AddComponent<LateGameFakeOutput>();
            output.Configure(false, true);
            output.ResetPuzzleObject();
            Assert.IsTrue(output.IsActive);
            Assert.AreEqual(1, output.Activations);
        }
        finally { Object.DestroyImmediate(go); }
    }
}
