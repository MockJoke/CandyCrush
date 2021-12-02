using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{

    public int rows, cols, TokenNo, TokenCnt;
    public GameObject Candies, BgBoard, BgBox, Tokens;
    public GameObject[] candy, token; 
    public GameObject[,] NewCandy;      //unmatched candies 
    MatchFinder matchFinder;
    public GameObject CurrentMovedCandy;


    // Start is called before the first frame update
    void Start()
    {
        matchFinder = FindObjectOfType<MatchFinder>();
        NewCandy = new GameObject[cols, rows];      //multidimensional array 

        CreateBoard();
    }
    

    void CreateBoard()
    {
        int TokenPosX = Random.Range(0, cols);
        int TokenPosY = rows - 1;

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int r = Random.Range(0, candy.Length);

                Vector2 pos = new Vector2(i, j);
                GameObject bg = Instantiate(BgBox, pos, Quaternion.identity);       //generate grid boxes 
                bg.transform.SetParent(BgBoard.transform);
                bg.name = "BG(" + i + "," + j + ")";

                while (CheckCandyPos(i, j, candy[r]))
                {
                    r = Random.Range(0, candy.Length);
                }

                GameObject GenCandy = Instantiate(candy[r], pos, Quaternion.identity);      //generate candies

                GenCandy.GetComponent<Candy>().row = j;
                GenCandy.GetComponent<Candy>().col = i;

                GenCandy.transform.SetParent(Candies.transform);
                GenCandy.name = "Candy(" + i + "," + j + ")";
                NewCandy[i, j] = GenCandy;
                
                  
            }
        }

        int t = Random.Range(0, token.Length);
        Destroy(NewCandy[TokenPosX, TokenPosY]);

        GameObject GenToken = Instantiate(token[t], new Vector2(TokenPosX, TokenPosY), Quaternion.identity);      //generate tokens

        GenToken.GetComponent<Candy>().row = TokenPosY;
        GenToken.GetComponent<Candy>().col = TokenPosX;

        GenToken.transform.SetParent(Tokens.transform);
        GenToken.name = "Token(" + TokenPosX + "," + TokenPosY + ")";

        TokenNo++; 
        

        NewCandy[TokenPosX, TokenPosY] = GenToken;
    }
    
    bool CheckCandyPos(int col, int row, GameObject GenCandy)       //to generate unmatched candies 
    {
        if (col > 1 && row > 1)
        {
            if (NewCandy[col - 1, row].tag == GenCandy.tag && NewCandy[col - 2, row].tag == GenCandy.tag)     //comparing with tags 
            {
                return true;
            }
            if (NewCandy[col, row - 1].tag == GenCandy.tag && NewCandy[col, row - 2].tag == GenCandy.tag)
            {
                return true;
            }
        }
        else
        {
            if (col > 1)
            {
                if (NewCandy[col - 1, row].tag == GenCandy.tag && NewCandy[col - 2, row].tag == GenCandy.tag)
                {
                    return true;
                }
            }
            else if (row > 1)
            {
                if (NewCandy[col, row - 1].tag == GenCandy.tag && NewCandy[col, row - 2].tag == GenCandy.tag)
                {
                    return true;
                }
            }
        }

        return false;

    }

    public void DestroyMatchedCandies()
    {
        if (matchFinder.CurrentMatchedCandies.Count == 4)
        {
            matchFinder.CheckForBomb();
        }

        if (matchFinder.CurrentMatchedCandies.Count >= 5)
        {
            matchFinder.CheckForColourBomb();
        }

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (NewCandy[i, j] != null)
                {
                    MatchedCandiesPos(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRows());

    }

    public void MatchedCandiesPos(int col, int row)
    {
        if (NewCandy[col, row].GetComponent<Candy>().CandyMatched)
        {
            Destroy(NewCandy[col, row]);
            NewCandy[col, row] = null;
        }
             
    }

    IEnumerator DecreaseRows()
    {
        int NullCnt = 0;

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (NewCandy[i, j] == null)
                {
                    NullCnt++;
                }
                else if (NullCnt > 0)
                {
                    NewCandy[i, j].GetComponent<Candy>().row -= NullCnt;
                    NewCandy[i, j] = null;
                }
            }

            NullCnt = 0;

        }

        yield return new WaitForSeconds(.7f);

        StartCoroutine(RefillBoard());

    }

    IEnumerator RefillBoard()
    {
        FillBoard();

        yield return new WaitForSeconds(0.6f);

        while (CheckAutoMatch())
        {
            yield return new WaitForSeconds(.7f);
            DestroyMatchedCandies();
        }

        matchFinder.CurrentMatchedCandies.Clear(); 

    }

    public void FillBoard()
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (NewCandy[i, j] == null)
                {
                    int r = Random.Range(0, candy.Length);

                    Vector2 pos = new Vector2(i, j);

                    GameObject GenCandy = Instantiate(candy[r], pos, Quaternion.identity);      //generate candies

                    GenCandy.GetComponent<Candy>().row = j;
                    GenCandy.GetComponent<Candy>().col = i;

                    GenCandy.transform.SetParent(Candies.transform);
                    GenCandy.name = "Candy(" + i + "," + j + ")";
                    NewCandy[i, j] = GenCandy;
                }
            }
        }

        matchFinder.FindAllMatches(); 

    }

    bool CheckAutoMatch()
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (NewCandy[i, j] != null)
                {
                    if (NewCandy[i, j].GetComponent<Candy>().CandyMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;

    }

    public void GenToken()
    {
        int TokenPosX = Random.Range(0, cols);
        int TokenPosY = rows - 1;

        int t = Random.Range(0, token.Length);
        Destroy(NewCandy[TokenPosX, TokenPosY]);

        GameObject GenToken = Instantiate(token[t], new Vector2(TokenPosX, TokenPosY), Quaternion.identity);      //generate tokens

        GenToken.GetComponent<Candy>().row = TokenPosY;
        GenToken.GetComponent<Candy>().col = TokenPosX;

        GenToken.transform.SetParent(Tokens.transform);
        GenToken.name = "Token(" + TokenPosX + "," + TokenPosY + ")";


        NewCandy[TokenPosX, TokenPosY] = GenToken;
    }

    public void ReloadBtn()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene("SampleScene");
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] ReceivedToken = GameObject.FindGameObjectsWithTag("token");
        if (ReceivedToken.Length == 0)
        {
            GenToken();
        }
    }
}
