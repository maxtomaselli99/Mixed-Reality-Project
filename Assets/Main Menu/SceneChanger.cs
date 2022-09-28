using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    [SerializeField] int Shooting_Scene, Bug_Scene;
    // Start is called before the first frame update

    public void ChangeToShooting()
    {
        SceneManager.LoadScene(Shooting_Scene);
    }

    public void ChangeToBugs()
    {
        SceneManager.LoadScene(Bug_Scene);

    }

    public void DummyEvent()
    {

    }
}
