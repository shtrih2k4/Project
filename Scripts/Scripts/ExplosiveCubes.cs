using UnityEngine;

public class ExplosiveCubes : MonoBehaviour
{
    private Rigidbody allcubesrb;
    public GameObject restartbutton;
    public GameObject explosion;
    [SerializeField]
    private float explosionForce = 70f;
    [SerializeField]
    private float explosionRadius = 5f;
    private bool collisionSet=false;
   
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<Rigidbody>(out allcubesrb)&&!collisionSet) { 
        
            if (PlayerPrefs.GetString("music") != "No")
            {
                GetComponent<AudioSource>()?.Play();
            }
            for (int i = collision.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, Vector3.up, explosionRadius);
                child.SetParent(null);
            }
            
            Destroy(collision.gameObject);
            collisionSet = true;
            GameObject expl=Instantiate(explosion, new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.contacts[0].point.z), Quaternion.identity) as GameObject;
            Destroy(expl, 1.5f);
            Camera.main.transform.localPosition -= new Vector3(0, 0, 3f);
            Camera.main.gameObject.AddComponent<CameraShaking>();
        }
    }
}
