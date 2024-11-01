using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropManipulator : PointerManipulator
{
    private readonly string containerId;
    private readonly string slotClassName;
    private readonly Action<VisualElement, Vector2, VisualElement> onDrop;

    public DragAndDropManipulator(VisualElement target, string containerId, string slotClassName, Action<VisualElement, Vector2, VisualElement> onDrop)
    {
        this.target = target;
        this.root = target.parent;
        this.containerId = containerId;
        this.slotClassName = slotClassName;
        this.onDrop = onDrop;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    private Vector2 targetStartPosition { get; set; }
    private Vector3 pointerStartPosition { get; set; }
    private bool enabled { get; set; }
    private VisualElement root { get; }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        targetStartPosition = target.transform.position;
        pointerStartPosition = evt.position;
        target.CapturePointer(evt.pointerId);
        enabled = true;
    }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            var pointerDelta = evt.position - pointerStartPosition;

            target.transform.position = new Vector2
            (
                Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height)
            );
        }
    }

    private void PointerUpHandler(PointerUpEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
        }
    }

    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        if (enabled)
        {
            if (root.TryGet<VisualElement>(this.containerId, out var slotsContainer))
            {
                var allSlots = slotsContainer.Query<VisualElement>(className: this.slotClassName);
                var overlappingSlots = allSlots.Where(this.OverlapsTarget);
                var closestElement = this.FindClosestSlot(overlappingSlots.ToList());

                var closestPosition = Vector3.zero;

                if (closestElement != null)
                {
                    closestPosition = this.RootSpaceOfSlot(closestElement);
                    closestPosition = new Vector2(closestPosition.x - 5, closestPosition.y - 5);
                }

                this.onDrop(closestElement, closestPosition, target);
            }

            enabled = false;
        }
    }

    private bool OverlapsTarget(VisualElement slot)
    {
        return target.worldBound.Overlaps(slot.worldBound);
    }

    private VisualElement FindClosestSlot(IList<VisualElement> slots)
    {
        var bestDistanceSq = float.MaxValue;
        var closest = default(VisualElement);

        foreach (var slot in slots)
        {
            var displacement = this.RootSpaceOfSlot(slot) - target.transform.position;

            var distanceSq = displacement.sqrMagnitude;
            if (distanceSq < bestDistanceSq)
            {
                bestDistanceSq = distanceSq;
                closest = slot;
            }
        }

        return closest;
    }

    private Vector3 RootSpaceOfSlot(VisualElement slot)
    {
        var slotWorldSpace = slot.parent.LocalToWorld(slot.layout.position);
        return root.WorldToLocal(slotWorldSpace);
    }
}