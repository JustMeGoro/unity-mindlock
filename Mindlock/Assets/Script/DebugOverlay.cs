using TMPro;
using UnityEngine;

public class DebugOverlay : MonoBehaviour
{

    public TMP_Text debugText; // pøipoj v Inspectoru
    public Rigidbody2D rb;

    // Update is called once per frame
    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        

        debugText.text =
            "Speed: " + speed.ToString("F2") + "\n" +
            "Horizontal: " + horizontal.ToString("F2") + "\n" +
            "Vertical: " + vertical.ToString("F2");
    }
}
