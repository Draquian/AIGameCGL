using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoManager : MonoBehaviour
{
    Vector3 auxPos;

    // Start is called before the first frame update
    void Start()
    {
        auxPos = transform.position;

        StartCoroutine(LookForPieces());
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Piece"))
        {
            Vector3 newPos = new Vector3(Random.Range(0, 9), 0.5f, Random.Range(0, 9));

            while (IsPieceAtTile(newPos))
            {
                newPos = new Vector3(Random.Range(0, 9), 0.5f, Random.Range(0, 9));
            }

            collision.gameObject.transform.position = newPos;
        }
    }

    IEnumerator LookForPieces()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                transform.position = new Vector3((auxPos.x - 1) + j, 1, (auxPos.z - 1) + i);

                yield return new WaitForSeconds(0.5f);
            }
        }
        Destroy(this.gameObject);
    }

    bool IsPieceAtTile(Vector3 pos)
    {
        ChessGameManager CGM = FindObjectOfType<ChessGameManager>();

        Collider[] colliders = Physics.OverlapSphere(pos, 0.5f, CGM.pieceLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Piece"))
            {
                return true;
            }
        }
        return false;
    }
}
