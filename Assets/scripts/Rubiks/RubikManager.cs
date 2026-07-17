using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class RubikManager : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private RubikInput inputManager;
    [SerializeField] private Transform cubiesParent;

    [Header("Config")]
    [SerializeField] private float rotationSpeed = 270f;

    [Header("Debug TRS")]
    [SerializeField] private List<FaceCenterData> centerMatrices = new List<FaceCenterData>();

    private List<Transform> allCubes = new List<Transform>();
    private bool isAnimating = false;

    [System.Serializable]
    public class FaceCenterData
    {
        public string faceName;
        public Transform cubieTransform;
        public Mat4x4 currentTRS;
    }

    void Start()
    {
        //Get all cubes
        foreach (Transform child in cubiesParent)
        {
            allCubes.Add(child);
        }

        IdentifyCenters();
    }

    void Update()
    {
        //update only if not animating (and have a new move)
        if (!isAnimating && inputManager.commandQueue.Count > 0)
        {
            RubikCommand cmd = inputManager.commandQueue.Dequeue();
            StartCoroutine(AnimateRotation(cmd));
        }

        //Update trs
        UpdateCenterMatrices();
    }

    private IEnumerator AnimateRotation(RubikCommand cmd)
    {
        isAnimating = true;

        List<Transform> activePieces = new List<Transform>();
        Vec3 centerOfRotation = cmd.axis * cmd.slice;

        //Find cubes in face
        foreach (var cubie in allCubes)
        {
            Vec3 localPos = new Vec3(cubie.localPosition.x, cubie.localPosition.y, cubie.localPosition.z);

            float dot = Vec3.Dot(localPos, cmd.axis);

            if ((cmd.slice > 0 && dot > 0.5f) || (cmd.slice < 0 && dot < -0.5f))
            {
                activePieces.Add(cubie);
            }
        }

        Vec3[] startPositions = new Vec3[activePieces.Count];
        Quat[] startRotations = new Quat[activePieces.Count];

        for (int i = 0; i < activePieces.Count; i++)
        {
            startPositions[i] = new Vec3(activePieces[i].localPosition.x, activePieces[i].localPosition.y, activePieces[i].localPosition.z);
            startRotations[i] = new Quat(activePieces[i].localRotation.x, activePieces[i].localRotation.y, activePieces[i].localRotation.z, activePieces[i].localRotation.w);
        }

        float duration = Mathf.Abs(cmd.angle) / rotationSpeed;
        float elapsed = 0f;

        Quat targetFaceRot = Quat.AngleAxis(cmd.angle, cmd.axis);

        //Animation
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = (elapsed / duration);

            Quat currentRotQuat = Quat.Slerp(Quat.identity, targetFaceRot, t);

            for (int i = 0; i < activePieces.Count; i++)
            {
                Vec3 offset = startPositions[i] - centerOfRotation;
                Vec3 rotatedOffset = currentRotQuat * offset;
                Vec3 finalPos = centerOfRotation + rotatedOffset;
                Quat finalRot = currentRotQuat * startRotations[i];

                activePieces[i].localPosition = new Vector3(finalPos.x, finalPos.y, finalPos.z);
                activePieces[i].localRotation = new Quaternion(finalRot.x, finalRot.y, finalRot.z, finalRot.w);
            }

            yield return null;
        }

        foreach (var piece in activePieces)
        {
            Vector3 p = piece.localPosition;
            piece.localPosition = new Vector3(Mathf.Round(p.x), Mathf.Round(p.y), Mathf.Round(p.z));

            Vector3 e = piece.localEulerAngles;
            piece.localEulerAngles = new Vector3(
                Mathf.Round(e.x / 90f) * 90f,
                Mathf.Round(e.y / 90f) * 90f,
                Mathf.Round(e.z / 90f) * 90f
            );
        }

        isAnimating = false;
    }

    private void IdentifyCenters()
    {
        // Center mag is always 1
        foreach (var cubie in allCubes)
        {
            Vec3 pos = new Vec3(cubie.localPosition.x, cubie.localPosition.y, cubie.localPosition.z);
            if (Mathf.Approximately(pos.sqrMagnitude, 1f))
            {
                centerMatrices.Add(new FaceCenterData
                {
                    faceName = $"Centro {cubie.name}",
                    cubieTransform = cubie
                });
            }
        }
    }

    private void UpdateCenterMatrices()
    {
        foreach (var center in centerMatrices)
        {
            Transform t = center.cubieTransform;

            Vec3 pos = new Vec3(t.position.x, t.position.y, t.position.z);
            Quat rot = new Quat(t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w);
            Vec3 scale = new Vec3(t.lossyScale.x, t.lossyScale.y, t.lossyScale.z);

            center.currentTRS = Mat4x4.TRS(pos, rot, scale);
        }
    }
}