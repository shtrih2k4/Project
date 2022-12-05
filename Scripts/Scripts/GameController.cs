using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{ 
    public float cubeChangePlaceSpeed = 0.5f;
    public GameObject allCubes, cube_to_create;
    public UnityEvent MouseClick;
    public float CamMoveSpeed = 2f;
    public GameObject[] InitialCanvas;
    public GameObject restartbutton;
    public GameObject VFX;
    private Vector3 nowCube=new (0,1,0);
    private List<Vector3> available_positions;
    private List<Vector3> covered_positions;

    
    //private CanvasButtons canvasbuttons;
    

    [SerializeField]private Transform cubeToPlace;
    private Coroutine showCubePlace;
    private Rigidbody allcubesRB;
    private Transform mainCamera;
    private bool isLost=false;
    private bool isStart=false;


    private float CamMoveToY;
    private float initialCameraY;

    private int prevCountMaxHorizontal;

    
    private void Awake()
    {
        available_positions = new();
        covered_positions = new();
        for(int xi = -1; xi <= 1; xi++)
        {
            for(int zi = -1; zi <= 1; zi++)
            {
                covered_positions.Add(new Vector3(nowCube.x+xi, nowCube.y-1, nowCube.z+zi));
            }
        }
        covered_positions.Add(nowCube);
        allcubesRB = allCubes.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        mainCamera = Camera.main.transform;
        initialCameraY = mainCamera.localPosition.y;
        CamMoveToY = initialCameraY+ nowCube.y - 1f;
        showCubePlace = StartCoroutine(ShowCubePlace());
    }
    private void Update()
    {
        LosingGame();
        GameControl();
        
        mainCamera.localPosition = Vector3.MoveTowards(mainCamera.localPosition, new Vector3(mainCamera.localPosition.x, CamMoveToY, mainCamera.localPosition.z), CamMoveSpeed * Time.deltaTime);
    }
    public void StartGame()
    {
        if (!isStart)
        {
            isStart = true;
            foreach(GameObject obj in InitialCanvas)
            {
                obj.SetActive(false);
            }
        }
    }
    public void GameControl()
    {
        if ((Input.GetMouseButtonDown(0)||Input.touchCount > 0)&&!EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began) return;  
#endif
            MouseClick?.Invoke();
        }
    }
    public void Cube_Instantiating() {
       
        if (cubeToPlace != null && cubeToPlace.position!=nowCube &&allCubes!=null)
        {
            if (PlayerPrefs.GetString("music") != "No")
            {
                GetComponent<AudioSource>().Play();
            }
            GameObject newcube = Instantiate(cube_to_create,
                    cubeToPlace.position,
                    Quaternion.identity) as GameObject;

            newcube.transform.SetParent(allCubes.transform);
            nowCube.Set(cubeToPlace.position.x, cubeToPlace.position.y, cubeToPlace.position.z);
            GameObject newVfx=Instantiate(VFX, newcube.transform.position, Quaternion.identity) as GameObject;
            Destroy(newVfx,1.5f);
            covered_positions.Add(nowCube);
            allcubesRB.WakeUp();
            MoveCamera();
            SpawnPositions();
            
        }
        
        
    }
    public void LosingGame()
    {
        if (!isLost && allcubesRB.velocity.magnitude > 0.03f)
        {
            StopCoroutine(showCubePlace);
            Destroy(cubeToPlace.gameObject);
            restartbutton.SetActive(true);
            isLost = true;
        }
        
    }
    private IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions(); 
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
       
    }
    private void SpawnPositions()
    {
        for (int i = -1; i <= 1; i++)
        {
            if (i == 0) continue;
            if (IsPositionEmpty(new Vector3(nowCube.x + i, nowCube.y, nowCube.z)) && nowCube.x + i != cubeToPlace.position.x) { available_positions.Add(new Vector3(nowCube.x + i, nowCube.y, nowCube.z)); }
            if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + i, nowCube.z)) && nowCube.y + i != cubeToPlace.position.y) { available_positions.Add(new Vector3(nowCube.x, nowCube.y + i, nowCube.z)); }
            if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + i)) && nowCube.z + i != cubeToPlace.position.z) { available_positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + i)); }

        }
        switch (available_positions.Count)
        {
            case > 1:
                cubeToPlace.position = available_positions[UnityEngine.Random.Range(0, available_positions.Count)];
                break;
            case 1:
                cubeToPlace.position = available_positions[0];
                break;
            case 0:
                isLost = true;
                restartbutton.SetActive(true);
                break;
            default:
                break;
        }
        available_positions.Clear();
    }
    private bool IsPositionEmpty(Vector3 pos) {
        foreach (Vector3 position in covered_positions)
        {
            if(position==pos||pos.y==0) { return false; }
        }
        return true;
    }
    private void MoveCamera()
    {
        int maxX=0,maxY=0, maxZ=0,maxHor;
        foreach(Vector3 pos in covered_positions)
        {
            if (Convert.ToInt32(Math.Abs(pos.x)) > maxX) maxX = Math.Abs(Convert.ToInt32(pos.x));
            if (Convert.ToInt32(pos.y) > maxY) maxY = Convert.ToInt32(pos.y);
            if (Convert.ToInt32(Math.Abs(pos.z)) > maxZ) maxZ = Math.Abs(Convert.ToInt32(pos.z));
        }

        CamMoveToY = initialCameraY + nowCube.y - 1f;
        maxHor=maxX>maxZ?maxX:maxZ;
        if(maxHor%3==0&&prevCountMaxHorizontal!=maxHor)
        {
            mainCamera.localPosition += new Vector3(0,0,-2f);
            prevCountMaxHorizontal = maxHor;
        }
    }
    
}

