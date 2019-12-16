using UnityEngine;

public class PageCol_Right : MonoBehaviour
{
    private Book book;

    // Start is called before the first frame update
    void Start()
    {
        book = this.gameObject.transform.parent.transform.parent.GetComponent<Book>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var hand = other.GetComponentInParent<FollowPivot>();

        if (hand.CompareTag("HandLeft") || hand.CompareTag("HandRight"))
            book.rightCollider = true;
    }

    private void OnTriggerExit(Collider other)
    {
        var hand = other.GetComponentInParent<FollowPivot>();

        if (hand.CompareTag("HandLeft") || hand.CompareTag("HandRight"))
            book.rightCollider = false;
    }
}
