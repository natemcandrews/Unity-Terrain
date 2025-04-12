using UnityEngine;

public class InteractiveRippleController : MonoBehaviour
{

    private Material material;
    private Color previousColor;

    private struct ShaderPropertyIDs
    {
        public int _BaseColor;
        public int _RippleColor;
        public int _RippleCenter;
        public int _RippleStartTime;
    }
    private ShaderPropertyIDs shaderProps;

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        material = Instantiate(renderer.sharedMaterial);
        renderer.material = material;

        shaderProps = new ShaderPropertyIDs()
        {
            _BaseColor = Shader.PropertyToID("_BaseColor"),
            _RippleColor = Shader.PropertyToID("_RippleColor"),
            _RippleCenter = Shader.PropertyToID("_RippleCenter"),
            _RippleStartTime = Shader.PropertyToID("_RippleStartTime"),
        };



        previousColor = material.GetColor(shaderProps._BaseColor);
        material.SetColor(shaderProps._RippleColor, previousColor);
    }

    private void OnDestroy()
    {
        if(material != null)
        {
            Destroy(material);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            CastClickRay();
        }
    }

    private void CastClickRay()
    {
        Camera camera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;

        Ray ray = camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, camera.nearClipPlane));

        if(Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject)
        {
            StartRipple(hit.point);
        }
    }

    private void StartRipple(Vector3 center)
    {
        Color rippleColor = Color.HSVToRGB(Random.value, 1, 1);

        material.SetVector(shaderProps._RippleCenter, center);
        material.SetFloat(shaderProps._RippleStartTime, Time.time);
        material.SetColor(shaderProps._BaseColor, previousColor);
        material.SetColor(shaderProps._RippleColor, rippleColor);

        previousColor = rippleColor;
    }
}
