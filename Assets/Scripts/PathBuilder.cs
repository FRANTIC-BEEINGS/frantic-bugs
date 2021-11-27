using System;
using System.Collections;
using System.Linq;
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

    void Start() {
        CanBuild = true;
        MouseState = MouseButtons.Nothing;
        OnCard = false;

        GameObject Map = GameObject.Find("Map");
        MapCardWidth = Map.GetComponent<MapGeneration>().MapCardWidth;
        MapCardHeight = Map.GetComponent<MapGeneration>().MapCardHeight;
        SelectedCards = new List<bool>();
        for (int i = 0; i < MapCardWidth * MapCardHeight; ++i) {
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
                if (PathBody.Count == 0) {
                    if (Candidate != null && Candidate.id == CurBody.id) {
                        Add();
                    }
                }
                else {
                    if (SelectedCards[CurBody.id]) {
                        //while (Path[Path.Count - 1].GetComponent<Card>().id != CurId) {
                        while (PathBody.Last().id != CurBody.id) {
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
                    }
                    Clear();
                }
            }
        }
    }

    bool Neighbor() {
        int NeighborId = PathBody.Last().id;
        int i1 = CurBody.id / MapCardWidth;
        int j1 = CurBody.id % MapCardWidth;
        int i2 = NeighborId / MapCardWidth;
        int j2 = NeighborId % MapCardWidth;
        return (i1 == i2 && j1 == j2 + 1) || (i1 == i2 && j1 == j2 - 1) || (i1 == i2 + 1 && j1 == j2) || (i1 == i2 - 1 && j1 == j2);
    }

    void Add() {
        SelectedCards[CurBody.id] = true;
        PathBody.Add(CurBody);
        CurBody.SetSelection(true);
    }

    void Remove() {
        PathBody.Last().SetSelection(false);
        SelectedCards[PathBody.Last().id] = false;
        PathBody.RemoveAt(PathBody.Count - 1);
    }

    void Clear() {
        while (PathBody.Count > 0) {
            Remove();
        }
    }

    void UpdateOnCard() {
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
