using Player;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public static bool walking;
    public AudioSource woodstep1;
    public AudioSource woodstep2;
    public AudioSource woodstep3;
    public AudioSource woodstep4;
    public AudioSource concretestep1;
    public AudioSource concretestep2;
    public AudioSource concretestep3;
    public AudioSource concretestep4;
    private int randomStep;
  private string surface = "";

    void Start()
    {
       walking = false; 
    }

    //behold the least optimised shit you'll ever see
    void Update()
    {
        if (walking && !woodstep1.isPlaying && !woodstep2.isPlaying && !woodstep3.isPlaying && !woodstep4.isPlaying && !concretestep1.isPlaying && !concretestep2.isPlaying && !concretestep3.isPlaying && !concretestep4.isPlaying)
        { 
            Debug.Log("Pinta on: " + surface);  
            randomStep = Random.Range(1, 5);
            switch (randomStep)
            {
                case 1:
                    if (surface == "WoodGround")
                    {
                        woodstep1.Play();
                    }
                    else if (surface == "ConcreteGround")
                    {
                        concretestep1.Play();
                    }
                    break;
                case 2:
                    if (surface == "WoodGround")
                    {
                        woodstep2.Play();
                    }
                    else if (surface == "ConcreteGround")
                    {
                        concretestep2.Play();
                    }
                    break;
                case 3:
                    if (surface == "WoodGround")
                    {
                        woodstep3.Play();
                    }
                    else if (surface == "ConcreteGround")
                    {
                        concretestep3.Play();
                    }
                    break;
                case 4:
                    if (surface == "WoodGround")
                    {
                        woodstep4.Play();
                    }
                    else if (surface == "ConcreteGround")
                    {
                        concretestep4.Play();
                    }
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WoodGround") || other.CompareTag("ConcreteGround"))
        {
            surface = other.tag;
        }
    }
}
