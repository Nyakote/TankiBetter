using UnityEngine;

public class M : MonoBehaviour
{
    public float x = 2;
    public int s = 100;
    public float t = 1;
    public int n = 1;
    public float time = 5;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = s;
        rb.maxAngularVelocity = s;
    }
    private void Update()
    {
        rb.maxLinearVelocity = s;
        rb.maxAngularVelocity = s;
        if (Input.GetKey("w")) x += n * Time.deltaTime;
        if (Input.GetKey("s")) x -= n * Time.deltaTime;
        if(Input.GetKey("a")) t -= n * Time.deltaTime;
        if(Input.GetKey("d")) t += n * Time.deltaTime;

        if(t>20) t = 20;
        if(t<-20) t = -20;
        if (!Input.GetKey("w") && !Input.GetKey("s") ) x = Mathf.Lerp(x,0,time *Time.deltaTime) ;
        if (!Input.GetKey("a") && !Input.GetKey("d") ) t = Mathf.Lerp(t,0, time * Time.deltaTime);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.forward*x, ForceMode.Acceleration);
        rb.AddTorque(Vector3.up * t, ForceMode.Acceleration);
    }
}
