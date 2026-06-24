using UnityEngine;

public class CharacterBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform hangPoint;

    [Header("Settings")]
    [SerializeField] private float scale = 1.5f;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Material characterMaterial;

    private GameObject[] bodyParts;

    private void Awake()
    {
        BuildCharacter();

        if (gameManager != null)
            gameManager.SetBodyParts(bodyParts);
    }

    private void BuildCharacter()
    {
        if (hangPoint == null)
        {
            Debug.LogError("HangPoint is not assigned.");
            return;
        }

        GameObject root = new GameObject("HangmanCharacter");

        // Главное место:
        root.transform.SetParent(hangPoint, false);
        root.transform.localPosition = offset;
        root.transform.localRotation = Quaternion.identity;
        root.transform.localScale = Vector3.one * scale;

        GameObject head = CreatePart(
            "Head",
            PrimitiveType.Sphere,
            root.transform,
            new Vector3(0f, -0.25f, 0f),
            Vector3.one * 0.45f,
            Quaternion.identity
        );

        GameObject body = CreatePart(
            "Body",
            PrimitiveType.Capsule,
            root.transform,
            new Vector3(0f, -1.05f, 0f),
            new Vector3(0.22f, 0.55f, 0.22f),
            Quaternion.identity
        );

        GameObject leftArm = CreatePart(
            "LeftArm",
            PrimitiveType.Capsule,
            root.transform,
            new Vector3(-0.42f, -0.95f, 0f),
            new Vector3(0.1f, 0.42f, 0.1f),
            Quaternion.Euler(0f, 0f, -55f)
        );

        GameObject rightArm = CreatePart(
            "RightArm",
            PrimitiveType.Capsule,
            root.transform,
            new Vector3(0.42f, -0.95f, 0f),
            new Vector3(0.1f, 0.42f, 0.1f),
            Quaternion.Euler(0f, 0f, 55f)
        );

        GameObject leftLeg = CreatePart(
            "LeftLeg",
            PrimitiveType.Capsule,
            root.transform,
            new Vector3(-0.22f, -1.95f, 0f),
            new Vector3(0.1f, 0.5f, 0.1f),
            Quaternion.Euler(0f, 0f, -20f)
        );

        GameObject rightLeg = CreatePart(
            "RightLeg",
            PrimitiveType.Capsule,
            root.transform,
            new Vector3(0.22f, -1.95f, 0f),
            new Vector3(0.1f, 0.5f, 0.1f),
            Quaternion.Euler(0f, 0f, 20f)
        );

        bodyParts = new GameObject[]
        {
            head,
            body,
            leftArm,
            rightArm,
            leftLeg,
            rightLeg
        };

        foreach (GameObject part in bodyParts)
            part.SetActive(false);
    }

    private GameObject CreatePart(
        string name,
        PrimitiveType type,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Quaternion localRotation
    )
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;

        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = localRotation;
        obj.transform.localScale = localScale;

        if (characterMaterial != null)
        {
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderer.material = characterMaterial;
        }

        return obj;
    }
}
