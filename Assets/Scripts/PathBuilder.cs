using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using Photon.Pun;
using UnityEngine.EventSystems;

enum MouseButtons {
    Left = 0,
    Right = 1,
    Nothing,
    Both
}

public class PathBuilder : MonoBehaviour
{
    public Action<List<Card>> pathBuilt;

    public bool CanBuild;

    [SerializeField] private MouseButtons MouseState;
    [SerializeField] private bool OnCard;

    [SerializeField] private BodyInformation Candidate;
    [SerializeField] private BodyInformation CurBody;

    private List<bool> SelectedCards;
    public List<BodyInformation> PathBody;
    public List<Card> PathCards;

    private int MapCardWidth, MapCardHeight;

    public MapGeneration Map;
    private bool initialized = false;

    void Update()
    {
        if (!initialized)
            return;
        UpdateOnCard();
        UpdateMouseState();
        UpdatePath();
    }

    public void Initialize(MapGeneration mapGeneration)
    {
        CanBuild = true;
        MouseState = MouseButtons.Nothing;
        OnCard = false;

        Map = mapGeneration;
        MapCardWidth = Map.GetMapCardWidth();
        MapCardHeight = Map.GetMapCardHeight();
        SelectedCards = new List<bool>();
        for (int i = 0; i < MapCardWidth * MapCardHeight; ++i) {
            SelectedCards.Add(false);
        }

        initialized = true;
    }

    void UpdatePath() {
        if (!CanBuild) {
            if (Candidate != null) {
                Candidate.SetHighlight(false);
            }
            Clear();
        }
        else {
            if (MouseState == MouseButtons.Right || MouseState == MouseButtons.Both) {
                Clear();
            }
            else if (MouseState == MouseButtons.Left && OnCard) {
                if (PathBody.Count == 0)
                {
                    Unit currentUnit = CurBody.gameObject.transform.parent.gameObject.GetComponent<Card>()
                        .GetCurrentUnit();
                    if (Candidate != null && Candidate.id == CurBody.id &&
                        currentUnit != null && currentUnit.gameObject.GetPhotonView().IsMine) {
                        Add(CurBody);
                    }
                }
                else {
                    if (SelectedCards[CurBody.id]) {
                        //while (Path[Path.Count - 1].GetComponent<Card>().id != CurId) {
                        Rollback(CurBody);
                    }
                    else {
                        BuildPathThroughNeighbors(PathBody.Last(), CurBody);
                        /*
                        if (Neighbor()) {
                            Add();
                        }
                        */
                    }
                }
            }
            else if (MouseState == MouseButtons.Nothing) {
                if (PathBody.Count == 0) {
                    if (OnCard && true) { // OnCard && на ней наш юнит
                        if (Candidate != null && Candidate.id != CurBody.id) {
                            Candidate.SetHighlight(false);
                        }
                        Candidate = CurBody;
                        Candidate.SetHighlight(true);
                    }
                    else {
                        if (Candidate != null) {
                            Candidate.SetHighlight(false);
                        }
                    }
                }
                else {
                    if (PathBody.Count > 1) {
                        PathCards.Clear();
                        for (int i = 0; i < PathBody.Count; ++i) {
                            PathCards.Add(PathBody[i].gameObject.transform.parent.gameObject.GetComponent<Card>());
                        }
                        // отправить построенный путь
                        pathBuilt?.Invoke(PathCards);
                        ClearPathBody();
                    }
                    else {
                        Clear();
                    }
                    //Clear();
                }
            }
        }
    }

    void BuildPathThroughNeighbors(BodyInformation Start, BodyInformation Finish) {
        BodyInformation Middle = Start;
        int i1 = Start.id / MapCardWidth;
        int j1 = Start.id % MapCardWidth;
        int i3 = Finish.id / MapCardWidth;
        int j3 = Finish.id % MapCardWidth;
        double angle = 0;
        if (i1 != i3 || j1 != j3) {
            angle = Math.Asin((j3 - j1) / Math.Sqrt((i1 - i3) * (i1 - i3) + (j1 - j3) * (j1 - j3)));
        }

        int di = (-1) * Convert.ToInt32(i1 > i3) + (1) * Convert.ToInt32(i1 < i3);
        int dj = (-1) * Convert.ToInt32(j1 > j3) + (1) * Convert.ToInt32(j1 < j3);

        while (true) {
            if (Neighbor(Middle, Finish)) {
                Add(Middle);
                Add(Finish);
                break;
            }
            int i2 = Middle.id / MapCardWidth;
            int j2 = Middle.id % MapCardWidth;

            Add(Middle);
            if (i2 == i3) {
                j2 += dj;
            }
            else if (j2 == j3) {
                i2 += di;
            }
            else {
                double anglei = Math.Asin((j3 - j2) / Math.Sqrt(((i2 + di) - i3) * ((i2 + di) - i3) + (j2 - j3) * (j2 - j3)));
                double anglej = Math.Asin((j3 - (j2 + dj)) / Math.Sqrt((i2 - i3) * (i2 - i3) + ((j2 + dj) - j3) * ((j2 + dj) - j3)));
                if (Math.Abs(anglei - angle) < Math.Abs(anglej - angle)) {
                    i2 += di;
                }
                else {
                    j2 += dj;
                }
            }

            Middle = Map.GetMap()[i2][j2].gameObject.transform.GetChild(0).GetComponent<BodyInformation>();
        }
    }

    bool Neighbor(BodyInformation Start, BodyInformation Finish) {
        int i1 = Start.id / MapCardWidth;
        int j1 = Start.id % MapCardWidth;
        int i2 = Finish.id / MapCardWidth;
        int j2 = Finish.id % MapCardWidth;
        return (i1 == i2 && j1 == j2 + 1) || (i1 == i2 && j1 == j2 - 1) || (i1 == i2 + 1 && j1 == j2) || (i1 == i2 - 1 && j1 == j2);
    }

    void Add(BodyInformation NewBody) {
        if (SelectedCards[NewBody.id] == false) {
            SelectedCards[NewBody.id] = true;
            PathBody.Add(NewBody);
            NewBody.SetSelection(true);
        }
        else {
            Rollback(NewBody);
        }
    }

    void Rollback(BodyInformation NewBody) {
        if (SelectedCards[NewBody.id] == true) {
            while (PathBody.Last().id != NewBody.id) {
                Remove();
            }
        }
    }

    void Remove() {
        PathBody.Last().SetSelection(false);
        SelectedCards[PathBody.Last().id] = false;
        PathBody.RemoveAt(PathBody.Count - 1);
    }

    void ClearPathBody() {
        while (PathBody.Count > 0) {
            SelectedCards[PathBody.Last().id] = false;
            PathBody.RemoveAt(PathBody.Count - 1);
        }
    }

    void Clear() {
        while (PathBody.Count > 0) {
            Remove();
        }
    }

    void UpdateOnCard() {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            OnCard = false;
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        OnCard = false;
        CurBody = null;
        if (Physics.Raycast(ray, out rayHit, 100.0f)) {
            if (rayHit.collider.tag == "Card") {
                OnCard = true;
                CurBody = rayHit.collider.transform.gameObject.GetComponent<BodyInformation>();
            }
        }
    }

    void UpdateMouseState() {
        if (Input.GetMouseButton((int)MouseButtons.Left)) {
            if (Input.GetMouseButton((int)MouseButtons.Right)) {
                MouseState = MouseButtons.Both;
            }
            else {
                MouseState = MouseButtons.Left;
            }
        }
        else {
            if (Input.GetMouseButton((int)MouseButtons.Right)) {
                MouseState = MouseButtons.Right;
            }
            else {
                MouseState = MouseButtons.Nothing;
            }
        }
    }
}
