using System;
using System.Collections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class KeyInHoleBehaviour : MonoBehaviour
{
    public Transform targetTransform;
    public Grabbable grabbable;
    public Rigidbody rb;
    public SnapInteractable keyHoleSnapInteractable;
    public HandGrabInteractable handGrabInteractable;
    public HandGrabInteractable HandGrabInteractable_mirror;
    public float moveTime = 1f;
    public float moveDistance = 0.05f;
    public float rotateTime = 1f;
    public float rotationAngle = 270f;

    public event EventHandler OnKeyInTheHole;

    public GameObject boxGrabInteractablesParent;


    public Transform boxLidTransform;
    public float boxLidRotationAngle = 15f;
    public float boxLidRotationTime = 0.5f;
    public AnimationCurve keyTranslationAnimCurve;
    public AnimationCurve keyRotationAnimCurve;
    public AnimationCurve boxLidAnimCurve;

    private bool coroutineExecuted;



    public void KeyInHole()
    {
        if (coroutineExecuted) return;

        StartCoroutine(MoveCoroutine());
    }


    private IEnumerator MoveCoroutine()
    {
        coroutineExecuted = true;
        yield return new WaitForSeconds(1f);


        // key is not grabbable anymore:
        grabbable.enabled = false;
        keyHoleSnapInteractable.enabled = false;
        handGrabInteractable.enabled = false;
        HandGrabInteractable_mirror.enabled = false;
        rb.isKinematic = true;

        // key translation in the lock
        Vector3 startPosition = targetTransform.position;
        Vector3 endPosition = startPosition + (targetTransform.forward * moveDistance);
        float lerp = 0f;
        while (lerp < 1)
        {
            yield return null;
            lerp += Time.deltaTime / moveTime;
            targetTransform.position = Vector3.Lerp(startPosition, endPosition, keyTranslationAnimCurve.Evaluate(lerp));
        }

        //key rotation in the lock
        Quaternion startRotation = targetTransform.rotation;
        Quaternion rotationDelta = Quaternion.AngleAxis(rotationAngle, targetTransform.forward);
        Quaternion endRotation = startRotation * rotationDelta;

        lerp = 0f;
        while (lerp < 1)
        {
            yield return null;
            lerp += Time.deltaTime / rotateTime;
            targetTransform.rotation = Quaternion.Slerp(startRotation, endRotation, keyRotationAnimCurve.Evaluate(lerp));
        }

        OnKeyInTheHole?.Invoke(this, EventArgs.Empty);


        // box Lid small rotation when the lock opens
        lerp = 0f;
        startRotation = boxLidTransform.rotation;
        rotationDelta = Quaternion.AngleAxis(boxLidRotationAngle, boxLidTransform.right);
        endRotation = startRotation * rotationDelta;
        while (lerp < 1)
        {
            yield return null;
            lerp += Time.deltaTime / boxLidRotationTime;
            boxLidTransform.rotation = Quaternion.Slerp(startRotation, endRotation, boxLidAnimCurve.Evaluate(lerp));
        }

        boxGrabInteractablesParent.SetActive(true);
    }
}
