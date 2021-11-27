using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

enum MouseButtons {
    Left = 0,
    Right = 1,
    Nothing,
    Both
}

public class PathBuilder : MonoBehaviour
{
    [SerializeField] public GameObject CandidatePrefab;
    [SerializeField] private GameObject Candidate;
    [SerializeField] public GameObject SelectionPrefab;
    [SerializeField] private MouseButtons MouseState;
    [SerializeField] private bool OnCard;
    [SerializeField] private GameObject CurCard;
    [SerializeField] private int CurId;
    private List<bool> SelectedCards;
    public List<GameObject> Path;
    public bool CanBuild;

    void Start() {
        CanBuild = true;
        MouseState = MouseButtons.Nothing;
        OnCard = false;
        GameObject Map = GameObject.Find("Map");

        SelectedCards = new List<bool>();
        for (int i = 0; i < Map.GetComponent<MapGeneration>().MapCardWidth * Map.GetComponent<MapGeneration>().MapCardHeight; ++i) {
            SelectedCards.Add(false);
        }
    }

    void Update() {
        UpdateOnCard();
        UpdateMouseState();
        UpdatePath();
    }

    void UpdatePath() {
        if (!CanBuild) {
            Clear();
        }
        else {
            if (MouseState == MouseButtons.Right || MouseState == MouseButtons.Both) {
                Clear();
            }
            else if (MouseState == MouseButtons.Left && OnCard) {
                if (Path.Count == 0) {
                    if (Candidate != null && Candidate.transform.position == CurCard.transform.position) {
                        Add();
                    }
                }
                else {
                    if (SelectedCards[CurId]) {
                        while (Path[Path.Count - 1].GetComponent<Card>().id != CurId) {
                            Remove();
                        }
                    }
                    else {
                        if (Neighbor()) {
                            Add();
                        }
                    }
                }
            }
            else if (MouseState == MouseButtons.Nothing) {
                if (Path.Count == 0) {
                    if (OnCard && true) { // OnCard && на ней наш юнит
                        Destroy(Candidate);
                        Candidate = Instantiate(CandidatePrefab, CurCard.transform.position, Quaternion.identity);
                    }
                    else {
                        Destroy(Candidate);
                    }
                }
                else {
                    if (Path.Count > 1) {
                        // отправить построенный путь
                    }
                    Clear();
                }
            }
        }
    }

    bool Neighbor() {
        GameObject Map = GameObject.Find("Map");
        int NeighborId = Path[Path.Count - 1].GetComponent<Card>().id;
        int i1 = CurId / Map.GetComponent<MapGeneration>().MapCardWidth;
        int j1 = CurId % Map.GetComponent<MapGeneration>().MapCardWidth;
        int i2 = Path[Path.Count - 1].GetComponent<Card>().id / Map.GetComponent<MapGeneration>().MapCardWidth;
        int j2 = Path[Path.Count - 1].GetComponent<Card>().id % Map.GetComponent<MapGeneration>().MapCardWidth;
        return (i1 == i2 && j1 == j2 + 1) || (i1 == i2 && j1 == j2 - 1) || (i1 == i2 + 1 && j1 == j2) || (i1 == i2 - 1 && j1 == j2);
    }

    void Add() {
        SelectedCards[CurId] = true;
        Path.Add(CurCard);
        GameObject NewSelection = Instantiate(SelectionPrefab, CurCard.transform.position, Quaternion.identity);
        NewSelection.transform.parent = CurCard.transform;
    }

    void Remove() {
        Destroy(Path[Path.Count - 1].transform.Find("CardSelection(Clone)").gameObject);
        SelectedCards[Path[Path.Count - 1].GetComponent<Card>().id] = false;
        Path.RemoveAt(Path.Count - 1);
    }

    void Clear() {
        while (Path.Count > 0) {
            Remove();
        }
    }

    void UpdateOnCard() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        OnCard = false;
        CurCard = null;
        CurId = -1;
        if (Physics.Raycast(ray, out rayHit, 100.0f)) {
            if (rayHit.collider.tag == "Card") {
                OnCard = true;
                CurCard = rayHit.collider.transform.parent.gameObject;
                CurId = CurCard.GetComponent<Card>().id;
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
