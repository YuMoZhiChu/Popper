/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class FairyControl : MonoBehaviour {

	// Add public variables here:
    public GameObject BuffParticle1;
	
	// Add private members here:
    private Transform[] destinationObjects;
    private int destinationsCount;
	
	// Add member functions here:
    void AddEffectToPlayer(GameObject hero)
    {
        SpeedUp spdU = hero.AddComponent<SpeedUp>();
        GameObject par = Instantiate(BuffParticle1, hero.transform.position + new Vector3(0, 6f, 0), Quaternion.identity) as GameObject;
        par.transform.SetParent(hero.transform);
        Destroy(par, spdU.persistenceTime);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerHero")
        {
            AddEffectToPlayer(other.gameObject);
            Destroy(this.gameObject);
            GameControl.instance.FairyReborn();
        }
    }

    void ChangeDestinationRandom()
    {
        int rnd = Random.Range(0, destinationsCount);
        //GetComponent<NavMeshAgent>().destination = destinationObjects[rnd].transform.position;
    }

    void PeriodChangeDestination()
    {
        ChangeDestinationRandom();
    }

	// Use this for initialization
	void Start () {
        destinationObjects = GameControl.instance.fairyLayer.FindChild("FairyDestinationObjects").GetComponentsInChildren<Transform>();
        destinationsCount = destinationObjects.Length;
        //ChangeDestinationRandom();
        InvokeRepeating("PeriodChangeDestination", 0, 15);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
