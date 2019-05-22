using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class LevelBehavior : MonoBehaviour 
{
    public int numberOfTypes = 0;
    public int numberOfPieces = 0;
    public int dropPieceHeight = 0;
    public int floors = 0;

    public Material currentPieceMaterial;
    public Material currentPieceMatchMaterial;

    private List<int> Pieces;

    private int[,] grid = new int[32, 32];

    private static string[] names = { "IBlock", "IIBlock", "LBlock", "OBlock", "TBlock", "ZBlock" };

    private static string[] clones = { "IBlock(Clone)", "IIBlock(Clone)", "LBlock(Clone)", "OBlock(Clone)", "TBlock(Clone)", "ZBlock(Clone)" };
    private int currentDropPiece = 0;
    private int currentFloor = 0;


    private float downStart;
    private double howLong = 0.4;
    private bool hasFired = false;
    private Vector3 gridPosition = new Vector3();

    private string beg = "pCube";
    private string[] masterArray = new string[804];

    private List<Vector3> chain = new List<Vector3>();
    private List<Vector3> destroyers = new List<Vector3>();

    private static List<Transform> matchingPieces = new List<Transform>();

    // Swiping
    private float minSwipeDistY;
    private float minSwipeDistX;
    private Vector2 startPos;    

    void Start()
    {
        Pieces = LevelPieces(numberOfTypes);

        GenerateLevel(numberOfPieces, Pieces);
        GenerateNextPiece();
    }

    public List<int> LevelPieces(int n)
    {
        List<int> pieces = new List<int>();

        while (n > 0)
        {
            if (!pieces.Contains(n))
            {
                pieces.Add(UnityEngine.Random.Range(0, 6));
                n--;
            }
        }

        return pieces;
    }

    void Update()
    {
        //#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, startPos.y, 0)).magnitude;
                    if (swipeDistVertical > minSwipeDistY)
                    {
                        float swipeValue = Mathf.Sign(touch.position.y - startPos.y);
                        if (swipeValue > 0)
                        {
                            //up swipe
                            RepositionMatrix(Vector3.back);
                            gridPosition = transform.position;
                        }
                        else if (swipeValue < 0)
                        {
                            //down swipe
                            RepositionMatrix(Vector3.forward);
                            gridPosition = transform.position;
                        }
                    }

                    float swipeDistHorizontal = (new Vector3(touch.position.x, 0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;
                    if (swipeDistHorizontal > minSwipeDistX)
                    {
                        float swipeValue = Mathf.Sign(touch.position.x - startPos.x);
                        if (swipeValue > 0)
                        {
                            //right swipe							
                            RepositionMatrix(Vector3.right);
                            gridPosition = transform.position;
                        }
                        else if (swipeValue < 0)
                        {
                            //left swipe							
                            RepositionMatrix(Vector3.left);
                            gridPosition = transform.position;
                        }
                    }
                    break;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////

        if (Input.GetKeyUp(KeyCode.UpArrow | KeyCode.DownArrow | KeyCode.DownArrow | KeyCode.RightArrow))
        {
            hasFired = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            hasFired = false;
            downStart = Time.time;

            RepositionMatrix(Vector3.back);
            gridPosition = transform.position;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            hasFired = false;
            downStart = Time.time;

            RepositionMatrix(Vector3.forward);
            gridPosition = transform.position;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            hasFired = false;
            downStart = Time.time;

            RepositionMatrix(Vector3.right);
            gridPosition = transform.position;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            hasFired = false;
            downStart = Time.time;

            RepositionMatrix(Vector3.left);
            gridPosition = transform.position;
        }

        ///////////////////////////////////////////////////////////////////////////////

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (WaitHowLong)
            {
                RepositionMatrix(Vector3.back);
                gridPosition = transform.position;
            }
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (WaitHowLong)
            {
                RepositionMatrix(Vector3.forward);
                gridPosition = transform.position;
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (WaitHowLong)
            {
                RepositionMatrix(Vector3.right);
                gridPosition = transform.position;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (WaitHowLong)
            {
                RepositionMatrix(Vector3.left);
                gridPosition = transform.position;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        // Place piece
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            UpdateAllTransforms();

            // Add initial dummy, first potential match drop
            chain = new List<Vector3>();
            chain.Add(new Vector3(-15, currentFloor, -15));

            while (chain.Count > 0)
            {
                PlaceAndDestroy();
                // Remove potential match from pattern chain
                chain.RemoveAt(0);
            }

            GenerateNextPiece();
        }
    }

    private void PlaceAndDestroy()
    {
        // Test for initial match, if no, then do nothing
        Vector3 matchPiece = new Vector3(chain[0].x, currentFloor, chain[0].z);
        if (IsBlockMatch(matchPiece))
        {
            Debug.Log("Match.");

            // Delete piece from game
            GameObject toDelete = MatchingPiece(matchPiece);
            Destroy(toDelete);
            matchingPieces.Remove(toDelete.transform);

            ChainPatternParadigm(matchPiece);
        }
        else
        {
            Debug.Log("No match.");
        }
    }

    public void GenerateNextPiece()
    {
        // Delete the main piece
        GameObject[] objs = GameObject.FindGameObjectsWithTag("NewBlockPiece");
        for (var i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }

        currentDropPiece = Pieces.GetRandomElement();

        GameObject obj = GameObject.Find(names[currentDropPiece]);
        GameObject copy = (GameObject)Instantiate(obj, new Vector3(-15, dropPieceHeight, -15), Quaternion.identity);
        copy.tag = "NewBlockPiece";
        copy.transform.parent = GameObject.Find("MainPiece").transform;
        copy.transform.localScale = new Vector3((float)1, (float)1, (float)1);
        copy.GetComponent<Renderer>().material = currentPieceMaterial;

        CurrentPieceMaterialParadigm();
        UpdateFloor();
    }

    private void CurrentPieceMaterialParadigm()
    {
        try
        {
            Vector3 centerPiece = new Vector3(-15, currentFloor, -15);

            GameObject[] objs = GameObject.FindGameObjectsWithTag("NewBlockPiece");
            Debug.Log(objs.Length);
            for (var i = 0; i < objs.Length; i++)
            {
                if (IsBlockMatch(centerPiece))
                {
                    Debug.Log("White material.");
                    objs[i].GetComponent<Renderer>().material = currentPieceMatchMaterial;
                }
                else
                {
                    Debug.Log("Black material.");
                    objs[i].GetComponent<Renderer>().material = currentPieceMaterial;
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void UpdateAllTransforms()
    {
        matchingPieces = GameObject.Find("TetrispherePieces").transform.GetComponentsInChildren<Transform>().Where(x => x.name == clones[currentDropPiece]).ToList();
    }

    private void UpdateFloor()
    {
        currentFloor = floors - 1;
    }

    private void ChainPatternParadigm(Vector3 piece)
    {
        foreach (Vector3 algorithm in PieceAlgorithms(piece))
        {
            bool isMatch = matchingPieces.Any(o => o.transform.position.x.ToString("F1") == algorithm.x.ToString("F1") && o.transform.position.z.ToString("F1") == algorithm.z.ToString("F1"));
            if (isMatch)
            {
                // Add match to pattern chain
                chain.Add(algorithm);
            }
        }
    }

    private List<Vector3> PieceAlgorithms(Vector3 piece)
    {
        List<Vector3> patterns = new List<Vector3>();

        // IBlock(Clone) pattern match 
        if (currentDropPiece == 0)
        {
            patterns.Add(new Vector3(0, 0, 3) + piece);

            patterns.Add(new Vector3(0, 0, -3) + piece);

            patterns.Add(new Vector3(-1, 0, 0) + piece);

            patterns.Add(new Vector3(1, 0, 0) + piece);
        }
        // IIBlock(Clone) pattern match 
        else if (currentDropPiece == 1)
        {
            patterns.Add(new Vector3(0, 0, 1) + piece);

            patterns.Add(new Vector3(0, 0, -1) + piece);

            patterns.Add(new Vector3(-3, 0, 0) + piece);

            patterns.Add(new Vector3(3, 0, 0) + piece);
        }
        // LBlock(Clone) pattern match 
        else if (currentDropPiece == 2)
        {
            patterns.Add(new Vector3(0, 0, 2) + piece);
            patterns.Add(new Vector3(-1, 0, 2) + piece);
            patterns.Add(new Vector3(1, 0, 1) + piece);

            patterns.Add(new Vector3(0, 0, -2) + piece);
            patterns.Add(new Vector3(-1, 0, -1) + piece);
            patterns.Add(new Vector3(1, 0, -2) + piece);

            patterns.Add(new Vector3(2, 0, 0) + piece);
            patterns.Add(new Vector3(2, 0, -1) + piece);

            patterns.Add(new Vector3(-2, 0, 0) + piece);
            patterns.Add(new Vector3(-2, 0, 1) + piece);
        }
        // OBlock(Clone) pattern match 
        else if (currentDropPiece == 3)
        {
            patterns.Add(new Vector3(0, 0, 2) + piece);

            patterns.Add(new Vector3(0, 0, -2) + piece);

            patterns.Add(new Vector3(-2, 0, 0) + piece);

            patterns.Add(new Vector3(2, 0, 0) + piece);
        }
        // TBlock(Clone) pattern match 
        else if (currentDropPiece == 4)
        {
            patterns.Add(new Vector3(0, 0, 2) + piece);
            patterns.Add(new Vector3(-1, 0, 2) + piece);
            patterns.Add(new Vector3(1, 0, 2) + piece);

            patterns.Add(new Vector3(0, 0, -2) + piece);
            patterns.Add(new Vector3(-1, 0, -2) + piece);
            patterns.Add(new Vector3(1, 0, -2) + piece);

            patterns.Add(new Vector3(3, 0, 0) + piece);
            patterns.Add(new Vector3(2, 0, 1) + piece);
            patterns.Add(new Vector3(2, 0, -1) + piece);

            patterns.Add(new Vector3(-3, 0, 0) + piece);
            patterns.Add(new Vector3(-2, 0, 1) + piece);
            patterns.Add(new Vector3(-2, 0, -1) + piece);
        }
        // ZBlock(Clone) pattern match 
        else if (currentDropPiece == 5)
        {
            // Bottom
            patterns.Add(new Vector3(-1, 0, 1) + piece);
            patterns.Add(new Vector3(0, 0, 2) + piece);
            patterns.Add(new Vector3(1, 0, 3) + piece);
            //Top
            patterns.Add(new Vector3(1, 0, -1) + piece);
            patterns.Add(new Vector3(0, 0, -2) + piece);
            patterns.Add(new Vector3(-1, 0, -3) + piece);
            //Left
            patterns.Add(new Vector3(2, 0, 2) + piece);
            patterns.Add(new Vector3(2, 0, 1) + piece);
            patterns.Add(new Vector3(2, 0, 0) + piece);
            //Right
            patterns.Add(new Vector3(-2, 0, -2) + piece);
            patterns.Add(new Vector3(-2, 0, -1) + piece);
            patterns.Add(new Vector3(-2, 0, 0) + piece);
        }

        return patterns;
    }

    public void GenerateLevel(int total, List<int> blocks)
    {
        for (int floor = 0; floor < floors; floor++)
        {
            // Clear grid
            ClearGrid();

            // Add games pieces
            for (var i = 0; i < total; i++)
            {
                string name = names[blocks.GetRandomElement()];
                GameObject obj = GameObject.Find(name);
                GameObject pieces = GameObject.Find("TetrispherePieces");

                float x = UnityEngine.Random.Range(0, 31);
                float z = UnityEngine.Random.Range(0, 31);

                int trys = 4;
                while (!IsOpenSpace(name, (int)x, (int)z))
                {
                    if (trys == 0)
                    {
                        name = "Block";
                        obj = GameObject.Find(name);

                        x = UnityEngine.Random.Range(0, 31);
                        z = UnityEngine.Random.Range(0, 31);
                    }     
                    else
                    {
                        name = names[blocks.GetRandomElement()];
                        obj = GameObject.Find(name);

                        x = UnityEngine.Random.Range(0, 31);
                        z = UnityEngine.Random.Range(0, 31);

                        trys--;
                    }
                }

                GameObject copy = (GameObject)Instantiate(obj, new Vector3(-x, floor, -z), Quaternion.identity);
                copy.transform.parent = pieces.transform;
                copy.transform.localScale = new Vector3((float)1, (float)1, (float)1);
            }

            // Add filler pieces
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (IsOpenSpace("Block", (int)i, (int)j))
                    {
                        GameObject obj = GameObject.Find("Block");
                        GameObject pieces = GameObject.Find("TetrispherePieces");

                        //Debug.Log(new Vector3(-i, floor, -j));

                        GameObject copy = (GameObject)Instantiate(obj, new Vector3(-i, floor, -j), Quaternion.identity);
                        copy.transform.parent = pieces.transform;
                        copy.transform.localScale = new Vector3((float)1, (float)1, (float)1);
                    }
                }
            }
        }
    }

    private void ClearGrid()
    {
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                grid[i, j] = 0;
            }
        }
    }

    private bool IsOpenSpace(string name, int x, int z)
    {
        try
        {
            if (name == names[0])
            {
                if (grid[x, z] == 0 && grid[x, z + 1] == 0 && grid[x, z + 2] == 0)
                {
                    grid[x, z] = 1;
                    grid[x, z + 1] = 1;
                    grid[x, z + 2] = 1;

                    return true;
                }
            }
            else if (name == names[1])
            {
                if (grid[x, z] == 0 && grid[x + 1, z] == 0 && grid[x + 2, z] == 0)
                {
                    grid[x, z] = 2;
                    grid[x + 1, z] = 2;
                    grid[x + 2, z] = 2;

                    return true;
                }
            }
            else if (name == names[2])
            {
                if (grid[x, z] == 0 && grid[x + 1, z] == 0 && grid[x, z + 1] == 0)
                {
                    grid[x, z] = 3;
                    grid[x + 1, z] = 3;
                    grid[x, z + 1] = 3;

                    return true;
                }
            }
            else if (name == names[3])
            {
                if (grid[x, z] == 0 && grid[x + 1, z] == 0 && grid[x, z + 1] == 0 && grid[x + 1, z + 1] == 0)
                {
                    grid[x, z] = 4;
                    grid[x + 1, z] = 4;
                    grid[x, z + 1] = 4;
                    grid[x + 1, z + 1] = 4;

                    return true;
                }
            }
            else if (name == names[4])
            {
                if (grid[x + 1, z] == 0 && grid[x, z + 1] == 0 && grid[x + 1, z + 1] == 0 && grid[x + 2, z + 1] == 0)
                {
                    grid[x + 1, z] = 5;
                    grid[x, z + 1] = 5;
                    grid[x + 1, z + 1] = 5;
                    grid[x + 2, z + 1] = 5;

                    return true;
                }

            }
            else if (name == names[5])
            {
                if (grid[x, z] == 0 && grid[x, z + 1] == 0 && grid[x + 1, z + 1] == 0 && grid[x + 1, z + 2] == 0)
                {
                    grid[x, z] = 6;
                    grid[x, z + 1] = 6;
                    grid[x + 1, z + 1] = 6;
                    grid[x + 1, z + 2] = 6;

                    return true;
                }
            }
            else if (name == "Block")
            {
                if (grid[x, z] == 0)
                {
                    grid[x, z] = 7;

                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        catch (System.IndexOutOfRangeException e)
        {

        }

        return false;
    }

    private void RepositionMatrix(Vector3 offset)
    {
        foreach (Transform obj in GameObject.Find("TetrispherePieces").transform)
        {
            try
            {
                Vector3 objPos = obj.transform.position;
                Vector3 newPos = objPos + offset;

                if (!OffGrid(newPos))
                {
                    //iTween.Defaults.easeType = iTween.EaseType.easeInQuad;
                    //iTween.MoveBy(obj.gameObject, offset, 1);
                    obj.transform.position += offset;
                }
                else
                {
                    if (newPos.x < -31)
                    {
                        newPos.x += 32;
                    }

                    if (newPos.z < -31)
                    {
                        newPos.z += 32;
                    }

                    if (newPos.x > 0)
                    {
                        newPos.x -= 32;
                    }

                    if (newPos.z > 0)
                    {
                        newPos.z -= 32;
                    }

                    //iTween.MoveTo(obj.gameObject, newPos, 1);
                    obj.transform.position = newPos;
                }
            }
            catch
            {

            }
        }

        UpdateAllTransforms();
        UpdateFloor();
        CurrentPieceMaterialParadigm();
    }

    private bool IsBlockMatch(Vector3 match)
    {
        return matchingPieces.Any(o => o.transform.position.x.ToString("F1") == match.x.ToString("F1") && o.transform.position.y.ToString("F1") == match.y.ToString("F1") && o.transform.position.z.ToString("F1") == match.z.ToString("F1"));
    }
    private GameObject MatchingPiece(Vector3 match)
    {
        return matchingPieces.Single(o => o.transform.position.x.ToString("F1") == match.x.ToString("F1") && o.transform.position.y.ToString("F1") == match.y.ToString("F1") && o.transform.position.z.ToString("F1") == match.z.ToString("F1")).gameObject;
    }    

    private bool OffGrid(Vector3 newPos)
    {
        Debug.Log(newPos);

        if ((newPos.x < -31) || (newPos.x > 0))
        {
            return true;
        }

        if ((newPos.z < -31) || (newPos.z > 0))
        {
            return true;
        }

        return false;
    }

    private bool WaitHowLong
    {
        get
        {
            if (!hasFired && (Time.time - downStart) >= howLong)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

public static class Extensions
{
    private static System.Random random = new System.Random();

    public static T GetRandomElement<T>(this IEnumerable<T> list)
    {
        // If there are no elements in the collection, return the default value of T
        if (list.Count() == 0)
            return default(T);

        return list.ElementAt(random.Next(list.Count()));
    }
}

public static class EnhancedMath
{
    private delegate double RoundingFunction(double value);

    private enum RoundingDirection { Up, Down }

    public static double RoundUp(double value, int precision)
    {
        return Round(value, precision, RoundingDirection.Up);
    }

    public static double RoundDown(double value, int precision)
    {
        return Round(value, precision, RoundingDirection.Down);
    }

    private static double Round(double value, int precision,
                RoundingDirection roundingDirection)
    {
        RoundingFunction roundingFunction;
        if (roundingDirection == RoundingDirection.Up)
            roundingFunction = Math.Ceiling;
        else
            roundingFunction = Math.Floor;
        value *= Math.Pow(10, precision);
        value = roundingFunction(value);
        return value * Math.Pow(10, -1 * precision);
    }
}
