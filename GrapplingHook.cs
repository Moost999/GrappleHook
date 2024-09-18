using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public Transform hookTip; // Ponto de onde o gancho é lançado
    public LayerMask grappleableLayer; // Camada dos objetos que podem ser agarrados
    public float maxDistance = 20f; // Distância máxima do gancho
    public float swingForce = 10f; // Força de balanço

    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    private SpringJoint springJoint;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Certifique-se que o personagem tem um Rigidbody
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        // Configurações básicas do LineRenderer para garantir visibilidade
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    void Update()
    {
        // Desenha uma linha de debug para verificar a direção do gancho
        Debug.DrawRay(hookTip.position, hookTip.forward * maxDistance, Color.red, 1f);

        if (Input.GetMouseButtonDown(0))
        {
            LaunchGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseGrapple();
        }

        if (springJoint != null)
        {
            UpdateGrappleLine();
            SwingMovement();
        }
    }

    void LaunchGrapple()
    {
        RaycastHit hit;
        // Teste inicial sem filtro de camada para garantir que o Raycast está funcionando
        if (Physics.Raycast(hookTip.position, hookTip.forward, out hit, maxDistance, grappleableLayer))
        {
            Debug.Log("Gancho colidiu com: " + hit.collider.name);

            grapplePoint = hit.point;

            springJoint = gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

            // Configurações da física da mola
            springJoint.maxDistance = distanceFromPoint * 0.8f;
            springJoint.minDistance = distanceFromPoint * 0.25f;
            springJoint.spring = 4.5f;
            springJoint.damper = 7f;

            lineRenderer.enabled = true;
        }
        else
        {
            Debug.Log("Nada foi atingido.");
        }
    }

    void ReleaseGrapple()
    {
        if (springJoint != null)
        {
            Destroy(springJoint);
            lineRenderer.enabled = false;
        }
    }

    void UpdateGrappleLine()
    {
        lineRenderer.SetPosition(0, hookTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    void SwingMovement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-hookTip.right * swingForce, ForceMode.Acceleration);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(hookTip.right * swingForce, ForceMode.Acceleration);
        }
    }
}
