using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Action_RotateWorld : MonoBehaviour
{
    [Header("Visual Object")]
    public Transform visualBeam;
    public DelayActivator nextFaceActivator;

    [Header("Player Control")]
    public GameObject playerVisuals; // Player model child object

    [Header("★ Key: Spawn Point")]
    // Drag in the location where you want the player to "land" after rotation (an empty object)
    public Transform targetSpawnPoint;

    [Header("Rotation Settings")]
    public Transform worldRoot;
    public CinemachineCamera wideCamera;
    public Vector3 axis = new Vector3(1, 0, 0);
    public float angle = 90f;
    public float duration = 1.5f;

    public void StartRotation()
    {
        StartCoroutine(RotationRoutine());
    }

    IEnumerator RotationRoutine()
    {
        // 1. --- Lock & Invisible ---
        GameManager.Instance.IsInteracting = true;
        GameManager.Instance.TogglePlayerControl(false);

        // Physics Freeze (prevent movement)
        Rigidbody rb = GameManager.Instance.player.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // Invisible (prevent visual glitches)
        if (playerVisuals) playerVisuals.SetActive(false);

        if (wideCamera) wideCamera.Priority = 20;

        // 2. --- Beam Disappear Animation ---
        if (visualBeam != null)
        {
            Vector3 originalScale = visualBeam.localScale;
            float t = 0f;
            while (t < 0.5f)
            {
                visualBeam.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / 0.5f);
                t += Time.deltaTime;
                yield return null;
            }
            visualBeam.gameObject.SetActive(false);
        }

        // 3. --- Rotate World ---
        GameObject hinge = new GameObject("TempHinge");
        hinge.transform.position = transform.position; // Rotate around the beam
        hinge.transform.rotation = Quaternion.identity;

        Transform originalParent = worldRoot.parent;
        worldRoot.SetParent(hinge.transform, true);

        Quaternion startRot = hinge.transform.rotation;
        Quaternion targetRot = startRot * Quaternion.AngleAxis(angle, axis);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            hinge.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hinge.transform.rotation = targetRot;

        // 4. --- Restore Structure ---
        worldRoot.SetParent(originalParent, true);
        Destroy(hinge);

        if (wideCamera) wideCamera.Priority = 0;

        // 5. --- ★★★ FIX: Father-Son Reunion Teleport + Forced Upright ★★★ ---
        if (targetSpawnPoint != null)
        {
            GameObject parentObj = GameManager.Instance.player; // Your empty parent object

            // 1. Go to child object to find CharacterController (the troublemaker)
            CharacterController childCC = parentObj.GetComponentInChildren<CharacterController>();
            Rigidbody childRB = parentObj.GetComponentInChildren<Rigidbody>();

            // 2. Disable them all (Knock them out!)
            if (childCC != null) childCC.enabled = false;
            if (childRB != null) childRB.isKinematic = true;

            // 3. Move parent object (Dad goes first)
            parentObj.transform.position = targetSpawnPoint.position;

            // ★★★ KEY MODIFICATION HERE: FORCE UPRIGHT ★★★
            // Only copy the Y-axis rotation (facing direction), forcing X and Z to 0.
            // This prevents the player from "lying flat" due to the rotated landing point.
            float targetY = targetSpawnPoint.eulerAngles.y;
            parentObj.transform.rotation = Quaternion.Euler(0, targetY, 0);

            // 4. Drag child object back to parent center
            // If Capsule fell down, its localPosition became (0, -100, 0) etc.
            // We reset it to zero to bring it back to dad's embrace.
            if (childCC != null)
            {
                childCC.transform.localPosition = Vector3.zero;
                childCC.transform.localRotation = Quaternion.identity;
            }

            Transform capsuleTransform = parentObj.transform.Find("PlayerCapsule");
            if (capsuleTransform != null)
            {
                capsuleTransform.localPosition = Vector3.zero;
                capsuleTransform.localRotation = Quaternion.identity;
            }

            // 5. Re-enable physics (Wake up!)
            if (childCC != null) childCC.enabled = true;
            if (childRB != null) childRB.isKinematic = false;

            Debug.Log("Teleport Complete! Upright Position: " + targetSpawnPoint.position);
        }

        // 6. --- Land & Visible ---
        if (playerVisuals) playerVisuals.SetActive(true);
        // Note: The variable 'rb' here refers to the Rigidbody on the Player parent object (if any),
        // usually we manipulate 'childRB' above, but keeping this for compatibility doesn't hurt.
        if (rb) rb.isKinematic = false;

        GameManager.Instance.TogglePlayerControl(true);
        GameManager.Instance.IsInteracting = false;

        // Activate next beam
        if (nextFaceActivator != null) nextFaceActivator.BeginTimer();
    }
}